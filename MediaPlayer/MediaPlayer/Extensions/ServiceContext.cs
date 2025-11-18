namespace MediaPlayer.Extensions;

using Helpers;
using MediaPlayer.Data.Factory;
using MediaPlayer.Data.Factory.Abstraction;
using MediaPlayer.ViewModels;

public static partial class ServiceContext
{
    #region Functions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <param name="application_root"></param>
    /// <returns></returns>
    public static WebApplicationBuilder InjectServices(
        this WebApplicationBuilder builder, IConfiguration? configuration, string application_root)
    {
        AppGenerator.Messages
            .Add(0, new MessageTemplate("The catalogue is empty. Use the upload form to add videos.", MessageTemplateType.Information));

        AppGenerator.Messages
            .Add(1, new MessageTemplate("Choose MP4 files to upload to the Video Catalogue.", MessageTemplateType.Information));

        AppGenerator.Messages
            .Add(2, new MessageTemplate("Select a video from the table to start playback.", MessageTemplateType.Information));

        AppGenerator.Messages
            .Add(3, new MessageTemplate("An error occurred whilst uploading file(s). Response code 413. Please try again.", MessageTemplateType.Warning));

        builder.Services.AddMemoryCache(options => ServiceHelper.GetCacheOptions(options));

        builder.Services.AddSingleton<ITokenStore>(TokenStore.Instance);

        return builder;
    }

    #endregion
}
