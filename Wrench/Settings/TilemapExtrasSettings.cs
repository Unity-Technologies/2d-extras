using RecipeEngine.Api.Settings;
using RecipeEngine.Api.Commands;
using RecipeEngine.Modules.Wrench.Models;
using RecipeEngine.Modules.Wrench.Settings;

namespace TilemapExtras.Cookbook.Settings;

public class TilemapExtrasSettings : AnnotatedSettingsBase
{
    // Path from the root of the repository where packages are located.
    readonly string[] PackagesRootPaths = {"."};

    // update this to list all packages in this repo that you want to release.
    Dictionary<string, PackageOptions> PackageOptions = new()
    {
        {
            "com.unity.2d.tilemap.extras",
            new PackageOptions()
            {
                PackJobOptions = new PackJobOptions()
                {
                    PrePackCommands = new List<Command>()
                    {
                        new Command("git clone $UNITY_2D_REPO_GIT --no-checkout ../.unity/2d"),
                        new Command("cd ../.unity/2d && git fetch origin $GIT_BRANCH"),
                        new Command("cd ../.unity/2d && rm -f .git/index.lock"),
                        new Command("cd ../.unity/2d && git checkout -f --detach FETCH_HEAD")
                    }
                },
                ReleaseOptions = new ReleaseOptions() { IsReleasing = true }
            }
        }
    };

    public TilemapExtrasSettings()
    {
        Wrench = new WrenchSettings(
            PackagesRootPaths,
            PackageOptions
        );      
    }

    public WrenchSettings Wrench { get; private set; }
}
