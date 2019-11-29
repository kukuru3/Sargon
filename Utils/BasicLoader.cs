using System;

namespace Sargon.Utils {

    /// <summary> This class is meant to encapsulate the Ur loader.</summary>
    public class BasicLoader : State {

        private string directory = "";

        public BasicLoader(string path) {
            this.directory = Ur.Filesystem.Folders.GetDirectory(path);
        }

        public event Action Complete;

        protected internal override void Initialize() {
            Register(Hooks.Initialize, Scan);
        }

        private void Scan() {

            var lq = new Ur.Filesystem.LoadingQueue();
            var foundAttrs = Ur.Typesystem.Finder.FromAllAssemblies().GetTypesWithAttribute<Ur.Filesystem.UrLoaderAttribute>();
            foreach (var kvp in foundAttrs) {
                lq.RegisterLoader(kvp.Value, kvp.Key.Extensions);
            }

            Console.WriteLine("Enqueueing directory: " + new System.IO.DirectoryInfo(directory).FullName + " with children");

            lq.EnqueueDirectory(directory, true);

            lq.LoadingUpdate += (args) => {
                var loader = args.Sender;
                switch (args.State) {
                    case Ur.Filesystem.LoadStates.Completed:
                        Game.Log("[COMPLETE] : " + args.Sender.Path, ConsoleColor.DarkCyan);
                        Context.Assets.HandleAssetLoaded(args.Sender, args.Sender.LoadedAssetItem);
                        break;
                    case Ur.Filesystem.LoadStates.Failure:
                        Game.Log("[  FAIL  ] : " + args.Sender.Path, ConsoleColor.DarkRed);
                        break;
                }
            };

            lq.AllTasksComplete += Complete;
            lq.Execute();
        }
    }
}
