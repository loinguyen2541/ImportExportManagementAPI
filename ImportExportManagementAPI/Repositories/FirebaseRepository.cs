using Firebase.Auth;
using Firebase.Storage;
using ImportExportManagementAPI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImportExportManagementAPI.Repositories
{
    public class FirebaseRepository
    {
        public async Task<string> GetFile(FirebaseSetting setting, string filename, string filePath)
        {
            var stream = File.Open(filePath, FileMode.Open);

            // Construct FirebaseStorage, path to where you want to upload the file and Put it there
            var auth = new FirebaseAuthProvider(new FirebaseConfig(setting.ApiKey));
            var login = await auth.SignInWithEmailAndPasswordAsync(setting.AuthEmail, setting.AuthPassword);
            var cancellation = new CancellationTokenSource();
            var task = new FirebaseStorage(
                setting.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(login.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("Reports")
                .Child("Statictis")
                .Child(filename)
                .PutAsync(stream);
            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
            stream.Dispose();
            File.Delete(filePath);
            return downloadUrl;
        }
    }
}
