using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using BlazorProgressiveFileUpload.Client;
using BlazorProgressiveFileUpload.Client.Shared;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.IO;

namespace BlazorProgressiveFileUpload.Client.Pages
{
	public partial class Index
    {

		[Inject]
		public HttpClient Client { get; set; }

		// Create a global variable that will be used by OnChooseFile and UploadAsync methods
		private IBrowserFile? _selectedFile = null;

		public void OnChooseFile(InputFileChangeEventArgs e)
		{
            // Get the selected file
            _selectedFile = e.File;
		}

		private long _uploaded = 0;
		private double _percentage = 0;
		// The method that will submit the file to the server
		public async Task SubmitFileAsync()
		{
			// Create a multipart form data content which will hold the key value of the file that must be of type StreamContent
			var content = new MultipartFormDataContent();

			if (_selectedFile == null)
				return;

			using (var fileStream = _selectedFile.OpenReadStream(5000000))
			{
                // and the 40096 which are 40KB per packet and the third argument which as a callback for the OnProgress event (u, p) are u = Uploaded bytes and P is the percentage
                var streamContent = new ProgressiveStreamContent(fileStream, 40096, (u, p) =>
                {
                    // Set the values of the _uploaded & _percentage fields to the value provided from the event
                    _uploaded = u;
                    _percentage = p;

                    // Call StateHasChanged() to notify the component about this change to re-render the UI
                    StateHasChanged();
                });

                // Add the streamContent with the name to the FormContent variable
                content.Add(streamContent, "file", _selectedFile.Name);

                // Submit the request
                var response = await Client.PostAsync("weatherforecast", content);
            }
          
		}
	}
}