using Android.Content.Res;
using CommunityToolkit.Maui;
using FFImageLoading.Maui;
using Microsoft.Extensions.Logging;
using Refit;
using SolarLab.AdvertBoard.Mobile.Presentation.Infrastructure.Http;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Globalization;

namespace SolarLab.AdvertBoard.Mobile.Presentation
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            builder
                .UseMauiApp<App>()
                .UseFFImageLoading()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});

    				Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) =>
    				{
    					if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
    					{
    						contentPanel.IsTabStop = true;
    					}
    				});
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                if (view is Entry)
                {
                    // Убираем нижнюю линию
                    handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

                    // Меняем цвет плейсхолдера
                    handler.PlatformView.SetHintTextColor(ColorStateList.ValueOf(Android.Graphics.Color.Gray));
                }
            });
#endif

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<FiltersPageModel>();
            builder.Services.AddTransient<AdvertDetailsPageModel>();
            builder.Services.AddTransient<LoginPageModel>();
            builder.Services.AddTransient<RegisterPageModel>();
            builder.Services.AddTransient<ProfilePageModel>();
            builder.Services.AddTransient<EditProfilePageModel>();
            builder.Services.AddTransient<PublishedUserAdvertsPageModel>();
            builder.Services.AddTransient<UserDraftsPageModel>();
            builder.Services.AddTransient<UserDraftDetailsPageModel>();
            builder.Services.AddTransient<AddDraftPageModel>();
            builder.Services.AddTransient<AddDraftImagesPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();

            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");

            builder.Services.AddHttpClient("BaseApi", client =>
            {
                client.BaseAddress = new Uri("http://10.0.2.2:8083/");
            });

            builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();
            builder.Services.AddTransient<IAdvertApiClient, AdvertApiClient>();
            builder.Services.AddTransient<IImageApiClient, ImageApiClient>();
            builder.Services.AddTransient<ICommentsApiClient, CommentsApiClient>();
            builder.Services.AddTransient<IUsersApiClient, UsersApiClient>();
            builder.Services.AddSingleton<ICategoryStore, CategoryStore>();
            builder.Services.AddSingleton<IAuthService, AuthService>();

            return builder.Build();
        }
    }
}
