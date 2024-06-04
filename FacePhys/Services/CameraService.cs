using Android.Graphics;
using Android.Hardware.Camera2.Params;
using Android.Hardware.Camera2;
using Android.Media;
using Android.Views;
using Java.Util.Concurrent;

namespace FacePhys.Services;

public class CameraService
{
    CameraDevice? _cameraDevice;
    ImageReader? _imageReader;

    public async Task<bool> CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        return status == PermissionStatus.Granted;
    }

    public event EventHandler<byte[]>? FrameCaptured;

    public void InitializeCamera()
    {
        var cameraManager = (CameraManager?)Android.App.Application.Context.GetSystemService(Android.Content.Context.CameraService);
        if (cameraManager != null)
        {
            var cameraList = cameraManager.GetCameraIdList();
            foreach (var cameraId in cameraList)
            {
                var characteristics = cameraManager.GetCameraCharacteristics(cameraId);
                var facing = characteristics.Get(CameraCharacteristics.LensFacing);
                if (facing != null && (int)facing == (int)LensFacing.Front)
                {
                    cameraManager.OpenCamera(cameraId, new CameraStateCallback(this), null);
                    break;
                }
            }
        }
    }

    public void StartCamera()
    {
        InitializeCamera();
    }

    public void StopCamera()
    {
        _cameraDevice?.Close();
        _cameraDevice = null;
    }

    private class CameraStateCallback(CameraService owner) : CameraDevice.StateCallback
    {
        public override void OnOpened(CameraDevice camera)
        {
            owner._cameraDevice = camera;
            owner.SetupCaptureSession();
        }

        public override void OnDisconnected(CameraDevice camera)
        {
            camera.Close();
            owner._cameraDevice = null;
        }

        public override void OnError(CameraDevice camera, CameraError error)
        {
            camera.Close();
            owner._cameraDevice = null;

            throw new Exception($"Camera error: {error}");
        }
    }

    private void SetupCaptureSession()
    {
        _imageReader = ImageReader.NewInstance(640, 480, ImageFormatType.Jpeg, 2);
        _imageReader.SetOnImageAvailableListener(new ImageAvailableListener(this), null);

        var captureRequestBuilder = _cameraDevice?.CreateCaptureRequest(CameraTemplate.Preview);
        captureRequestBuilder?.AddTarget(_imageReader.Surface);

        var surfaces = new List<Surface> { _imageReader.Surface };
        var outputConfiguration = surfaces.Select(surface => new OutputConfiguration(surface)).ToList();
        var executor = Executors.NewSingleThreadExecutor();

        SessionConfiguration sessionConfiguration = new(
            (int)SessionType.Regular,
            outputConfiguration,
            executor,
            new SessionStateCallback(this)
        );

        _cameraDevice?.CreateCaptureSession(sessionConfiguration);
    }

    private class SessionStateCallback(CameraService owner) : CameraCaptureSession.StateCallback
    {
        public override void OnConfigured(CameraCaptureSession session)
        {
            try
            {
                owner.StartCapture(session);
            }
            catch (CameraAccessException e)
            {
                throw new Exception("Camera access exception", e);
            }
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            throw new Exception("Camera configuration failed");
        }
    }

    private void StartCapture(CameraCaptureSession session)
    {
        var captureRequestBuilder = _cameraDevice?.CreateCaptureRequest(CameraTemplate.Preview);
        captureRequestBuilder?.AddTarget(_imageReader.Surface);
        session.SetRepeatingRequest(captureRequestBuilder?.Build(), null, null);
    }

    private class ImageAvailableListener(CameraService owner) : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        public void OnImageAvailable(ImageReader reader)
        {
            try
            {
                var image = reader.AcquireLatestImage();
                if (image != null)
                {
                    var buffer = image.GetPlanes()[0].Buffer;
                    var bytes = new byte[buffer.Remaining()];
                    buffer.Get(bytes);
                    owner.FrameCaptured?.Invoke(owner, bytes);
                    image.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error acquiring image", e);
            }
        }
    }
}
