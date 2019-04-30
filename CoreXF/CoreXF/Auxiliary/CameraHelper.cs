
using Acr.UserDialogs;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;

namespace CoreXF
{
    public static class CameraHelper
    {

        public static async Task<bool> CheckCameraPermissions(bool ScanQR = false)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await UserDialogs.Instance.AlertAsync(ScanQR ? Tx.T("CoreXF_DevicePermission_QRScanRequeresCamera") : Tx.T("CoreXF_DevicePermission_AppNeedsCameraAccessForTakeAPhoto"));
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                status = results[Permission.Camera];
            }

            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else if (status != PermissionStatus.Unknown)
            {
                await UserDialogs.Instance.AlertAsync(Tx.T("CoreXF_DevicePermission_AppDoesntHaveAccessToCamera"));
            }

            return false;
        }


        /*
        public static async Task<bool> CheckCameraPermissions(bool qr = false)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await UserDialogs.Instance.AlertAsync(qr ? Tx.T("CoreXF_camerahelper_needaccess_qr") : Tx.T("CoreXF_camerahelper_needaccess"));
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                status = results[Permission.Camera];
            }

            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else if (status != PermissionStatus.Unknown)
            {
                await UserDialogs.Instance.AlertAsync(Tx.T("CoreXF_camerahelper_noaccess"));
            }

            return false;
        }
        */
        /*
        public static async Task<bool> CheckGalleryPermissions()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await UserDialogs.Instance.AlertAsync(qr ? Tx.T("CoreXF_camerahelper_needaccess_qr") : Tx.T("CoreXF_camerahelper_needaccess"));
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                status = results[Permission.Camera];
            }

            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            else if (status != PermissionStatus.Unknown)
            {
                await UserDialogs.Instance.AlertAsync(Tx.T("CoreXF_camerahelper_noaccess"));
            }

            return false;
        }
        */
        public static async Task<MediaFile> PickPhoto()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await UserDialogs.Instance.AlertAsync(":( Permission not granted to photos.");
                return null;
            }
            var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
            {
                PhotoSize = PhotoSize.Small
            });
            return file;
        }

        public static async Task<MediaFile> TakePhoto(PhotoSize photoSize = PhotoSize.Medium)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await UserDialogs.Instance.AlertAsync(":( No camera avaialble.");
                return null;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = photoSize,
                //Directory = "Sample",
                //Name = "test.jpg"
            });

            return file;
        }

        /*
        public static async Task<MediaFile> TakePhoto()
        {
            if(await CheckCameraPermissions(qr:false))
            {
                await CrossMedia.Current.Initialize();

                StoreCameraMediaOptions options = new StoreCameraMediaOptions()
                {
                    PhotoSize = PhotoSize.Small
                };

                var file = await CrossMedia.Current.TakePhotoAsync(options);
                return file;
            }
            return null;
        }
        */
    }
}
