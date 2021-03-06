﻿using Sargon.Graphics;
using System;
using Int2 = Ur.Grid.Coords;

namespace Sargon.Assets {
    public sealed class Texture : IAsset {
        #region Fields
        Int2 Size;
        private SFML.Graphics.Texture nativeTexture;
        private SFML.Graphics.RenderTexture nativeRenderTexture;
        #endregion

        #region Properties       
        public LoadStates LoadState { get; private set; }
        public bool IsNative { get; private set; }
        public string Path { get; private set; }
        public bool IsRenderTex { get; private set; }
        #endregion

        #region Property getters
        public int Width => Size.X;
        public int Height => Size.Y;
        internal SFML.Graphics.Texture NativeTexture => IsRenderTex ? nativeRenderTexture.Texture : nativeTexture;
        #endregion                                                                                                

        #region Ctors
        public Texture(string filepath) {
            Path = filepath;
            IsRenderTex = false;
            StartLoad();
        }

        internal Texture(int width, int height, Ur.Color color) {
            this.Path = "";
            this.nativeTexture = null;
            this.nativeRenderTexture = new SFML.Graphics.RenderTexture((uint)width, (uint)height, false);
            this.nativeRenderTexture.Clear(color.ToSFMLColor());
            this.Size = new Int2(width, height);
            IsRenderTex = true;
            StartLoad();
        }
        #endregion

        #region Public interface
        public void StartLoad() {

            if (nativeTexture != null) {
                Unload();
            }

            LoadState = LoadStates.Loading;

            try {

                if (IsRenderTex) {
                    Path = "";
                    // render textures expect size already set here.
                    nativeTexture = new SFML.Graphics.Texture((uint)Size.X, (uint)Size.Y);
                } else {
                    nativeTexture = new SFML.Graphics.Texture(Path);
                }
                nativeTexture.Smooth = true;
                nativeTexture.Repeated = true;
                Size = new Int2((int)nativeTexture.Size.X, (int)nativeTexture.Size.Y);
            } catch (Exception) { // pokemon exception catching :P                
                LoadState = LoadStates.Failed;
            }
            LoadState = LoadStates.Active;

        }

        public void Unload() {
            LoadState = LoadStates.NotLoaded;
            Size = new Int2();
        }

        public void Clear() {
            if (!IsRenderTex) throw new Errors.SargonException("Cannot clear nonrender texture");
            if (nativeRenderTexture != null) {
                nativeRenderTexture.Clear(Ur.Color.Black.ToSFMLColor());
            }

        }

        public void Dispose() {
            nativeTexture.Dispose();
            nativeRenderTexture?.Dispose();
        }
        #endregion

    }
}
