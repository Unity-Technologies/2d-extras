using RecipeEngine.Api.Settings;
using RecipeEngine.Api.Commands;
using RecipeEngine.Modules.Wrench.Models;
using RecipeEngine.Modules.Wrench.Settings;

namespace TilemapExtras.Cookbook.Settings;

public class TilemapExtrasSettings : AnnotatedSettingsBase
{
    // Path from the root of the repository where packages are located.
    readonly string[] PackagesRootPaths = {".", ".tests/Packages/com.unity.2d.tilemap.extras.tests"};

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
                        new Command("git clone $UNITY_2D_REPO_GIT --no-checkout .tests"),
                        new Command("cd .tests && git fetch origin $GIT_BRANCH"),
                        new Command("cd .tests && rm -f .git/index.lock"),
                        new Command("cd .tests && git checkout -f --detach FETCH_HEAD"),
                    },
                },
                ReleaseOptions = new ReleaseOptions() { IsReleasing = true }
            }
        },
        {
            "com.unity.2d.tilemap.extras.tests",
            new PackageOptions()
            {
                PackJobOptions = new PackJobOptions()
                {
                    PrePackCommands = new List<Command>()
                    {
                        new Command("git clone $UNITY_2D_REPO_GIT --no-checkout .tests"),
                        new Command("cd .tests && git fetch origin $GIT_BRANCH"),
                        new Command("cd .tests && rm -f .git/index.lock"),
                        new Command("cd .tests && git checkout -f --detach FETCH_HEAD"),
                    },
                },
                ReleaseOptions = new ReleaseOptions() { IsReleasing = false, NeverPublish = true }
            }
        },
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
