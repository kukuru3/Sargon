using Sargon.Assets;
using System;
using System.Collections.Generic;

namespace Sargon.Utils {

    /// <summary> This class is meant to encapsulate the Ur loader.</summary>
    public class BasicLoader {

        private string directory = "";

        List<IAsset> loadedAssets;

        public IEnumerable<IAsset> AllLoadedAssets => loadedAssets;

        public BasicLoader(string path) {
            this.directory = Ur.Filesystem.Folders.GetDirectory(path);
        }

        public event Action Complete;

        public void Scan() {

            loadedAssets = new List<IAsset>();

            var lq = new Ur.Filesystem.LoadingQueue();
            var foundAttrs = Ur.Typesystem.Finder.FromAllAssemblies().GetTypesWithAttribute<Ur.Filesystem.UrLoaderAttribute>();
            foreach (var kvp in foundAttrs) {
                lq.RegisterLoader(kvp.Value, kvp.Key.Extensions);
            }

            GameContext.Current.Logger.Add("Enqueueing directory: " + new System.IO.DirectoryInfo(directory).FullName + " with children");

            lq.EnqueueDirectory(directory, true);

            lq.LoadingUpdate += (args) => {
                var loader = args.Sender;
                switch (args.State) {
                    case Ur.Filesystem.LoadStates.Completed:
                        GameContext.Current.GameInstance.Log("[COMPLETE] : " + args.Sender.Path, ConsoleColor.DarkCyan);
                        GameContext.Current.Assets.HandleAssetLoaded(args.Sender, args.Sender.LoadedAssetItem, args.Sender.LoadedAssetMetadata);
                        if (args.Sender.LoadedAssetItem is IAsset asset) loadedAssets.Add(asset);
                        break;
                    case Ur.Filesystem.LoadStates.Failure:
                        GameContext.Current.GameInstance.Log("[  FAIL  ] : " + args.Sender.Path, ConsoleColor.DarkRed);
                        break;
                }
            };

            lq.AllTasksComplete += Complete;
            lq.Execute();
        }
    }
}
