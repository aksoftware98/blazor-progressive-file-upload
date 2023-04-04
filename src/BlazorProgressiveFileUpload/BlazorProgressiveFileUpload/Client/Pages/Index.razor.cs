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

namespace BlazorProgressiveFileUpload.Client.Pages
{
	public partial class Index
    {

		[Inject]
		public HttpClient Client { get; set; } = default!;

		// Create a global variable that will be used by OnChooseFile and UploadAsync methods
		private Stream? _fileStream = null;
		private string? _selectedFileName = null;

		public void OnChooseFile(InputFileChangeEventArgs e)
		{
			// Get the selected file
			var file = e.File;

			// Check if the file is null then return from the method
			if (file == null)
				return;

			// Validate the extension if required Client-Side)

			// Set the value of the stream by calling OpenReadStream and pass the maximum number of bytes allowed because by default it only allows 512KB
			// I used the value 5000000 which is about 50MB
			using (var stream = file.OpenReadStream(50000000))
			{
				_fileStream = stream;
				_selectedFileName = file.Name;
			}
		}

		private long _uploaded = 0;
		private double _percentage = 0;
		// The method that will submit the file to the server
		public async Task SubmitFileAsync()
		{
			// Create a multipart form data content which will hold the key value of the file that must be of type StreamContent
			var content = new MultipartFormDataContent();

			// Create an instance of ProgressiveStreamContent that we just created and we will pass the stream of the file for it
			// and the 40096 which are 40KB per packet and the third argument which as a callback for the OnProgress event (u, p) are u = Uploaded bytes and P is the percentage
			var streamContent = new ProgressiveStreamContent(_fileStream, 40096, (u, p) =>
			{
				// Set the values of the _uploaded & _percentage fields to the value provided from the event
				_uploaded = u;
				_percentage = p;

				// Call StateHasChanged() to notify the component about this change to re-render the UI
				StateHasChanged();
			});

			// Add the streamContent with the name to the FormContent variable
			content.Add(streamContent, "File");

			// Submit the request
			var response = await Client.PostAsync("/weatherforecast", streamContent);
		}
	}
}