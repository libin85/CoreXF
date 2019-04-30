using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using Photos;
using UIKit;
using Xamarin.Forms;

// https://stackoverflow.com/questions/38059953/how-to-fetch-all-photos-from-library
[assembly: Dependency(typeof(CoreXF.iOS.GalleryFilesService))]
namespace CoreXF.iOS
{
    public class GalleryFilesService : IGalleryService
    {
        public void GetFiles()
        {
            //PHAssetCollection

            var fetchOptions = new PHFetchOptions();
            fetchOptions.FetchLimit = 20;

            using (PHFetchResult allPhotos = PHAsset.FetchAssets(PHAssetMediaType.Image, fetchOptions))
            {
                foreach(PHAsset elm in allPhotos)
                {
                    var i = 4;
                }
            }

                /*
            var fetchOptions = PHFetchOptions();
            let allPhotos = PHAsset.fetchAssets(with: .image, options: fetchOptions)
        print("Found \(allPhotos.count) assets")
        */
        }

        [Preserve]
        public static void Init() { }
    }
}