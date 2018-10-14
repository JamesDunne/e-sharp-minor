using System;
using System.Collections.Generic;
using System.Diagnostics;
using EMinor.UI;
using OpenVG;
using Shapes;

namespace EMinor
{
    public class VGUI : IDisposable
    {
        private readonly Controller controller;
        private readonly IPlatform platform;
        private readonly IOpenVG vg;
        private readonly DisposalContainer disposalContainer;
        private readonly PaintColor clrBtnOutline;
        private readonly PaintColor clrBtnBg;
        private readonly PaintColor clrBtnOutlineSelected;
        private readonly PaintColor white;
        private readonly PaintColor pointColor;
        private readonly Ellipse point;
        private readonly FontHandle vera;
        private readonly Component root;

        private Component selectedComponent;

        public struct FootSwitchMapping
        {
            public Action Left;
            public Action Right;
        }

        private static readonly Func<Controller, FootSwitchMapping> FootSwitchScene = (controller) => new FootSwitchMapping
        {
            Left = controller.PreviousScene,
            Right = controller.NextScene
        };

        private static readonly Func<Controller, FootSwitchMapping> FootSwitchSong = (controller) => new FootSwitchMapping
        {
            Left = controller.PreviousSong,
            Right = controller.NextSong
        };
        private readonly PaintColor clrBtnBgSelected;
        private FootSwitchMapping footswitchMapping;

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            vg = platform.VG;
            footswitchMapping = FootSwitchScene(controller);
            selectedComponent = null;

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            platform.InputEvent += Platform_InputEvent;

            // Load TTF font:
            Debug.WriteLine("Load Vera.ttf");
            NRasterizer.Typeface typeFace;
            using (var fi = System.IO.File.OpenRead("Vera.ttf"))
            {
                Debug.WriteLine("OpenTypeReader");
                typeFace = new NRasterizer.OpenTypeReader().Read(fi);
            }

            Debug.WriteLine("Set rendering quality and pixel layout");
            vg.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_BETTER);
            vg.Seti(ParamType.VG_PIXEL_LAYOUT, (int)PixelLayout.VG_PIXEL_LAYOUT_RGB_HORIZONTAL);

            vera = vg.CreateFont(typeFace.Glyphs.Count);
            var vgRasterizer = new VGGlyphRasterizer(vg);
            vgRasterizer.ConvertGlyphs(typeFace, vera);

            this.disposalContainer = new DisposalContainer(
                white = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                // For the touch point:
                pointColor = new PaintColor(vg, new float[] { 0.0f, 1.0f, 0.0f, 0.5f }),
                point = new Ellipse(vg, 24, 24),
                clrBtnOutline = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                clrBtnBg = new PaintColor(vg, new float[] { 0.3f, 0.3f, 0.3f, 1.0f }),
                clrBtnBgSelected = new PaintColor(vg, new float[] { 0.3f, 0.3f, 0.6f, 1.0f }),
                clrBtnOutlineSelected = new PaintColor(vg, new float[] { 1.0f, 1.0f, 0.0f, 1.0f })
            );

            // Root of component tree:
            this.root = new Panel(platform)
            {
                Children = {
                    new VerticalStack(platform) {
                        Children = {
                            new HorizontalStack(platform) {
                                Dock = Dock.Top,
                                Height = 32,
                                Children = {
                                    // RESET button:
                                    new Button(platform) {
                                        Dock = Dock.Left,
                                        Width = 80,
                                        Stroke = clrBtnOutline,
                                        Fill = clrBtnBg,
                                        StrokePressed = clrBtnBg,
                                        FillPressed = clrBtnOutline,
                                        OnRelease = (cmp, p) => {
                                            controller.MidiReset();
                                            return true;
                                        },
                                        Children = {
                                            new Label(platform)
                                            {
                                                TextFont = vera,
                                                TextColor = white,
                                                Text = () => "RESET"
                                            }
                                        }
                                    },
                                    // Song display:
                                    new Button(platform) {
                                        Stroke = clrBtnOutline,
                                        Fill = clrBtnBg,
                                        StrokeLineWidth = 3.0f,
                                        OnPress = (cmp, p) => {
                                            var btn = (Button)cmp;

                                            // Footswitch controls song prev/next.
                                            footswitchMapping = FootSwitchSong(controller);
                                            if (selectedComponent is Button selectedBtn) {
                                                selectedBtn.Stroke = clrBtnOutline;
                                                selectedBtn.Fill = clrBtnBg;
                                            }
                                            btn.Stroke = clrBtnOutlineSelected;
                                            btn.Fill = clrBtnBgSelected;
                                            selectedComponent = cmp;
                                            return true;
                                        },
                                        Children = {
                                            new Label(platform)
                                            {
                                                TextFont = vera,
                                                TextColor = white,
                                                Text = () => $"SONG: {controller.CurrentSongName}"
                                            }
                                        }
                                    },
                                    // Scene display:
                                    (selectedComponent = new Button(platform) {
                                        Dock = Dock.Right,
                                        Width = 138,
                                        Stroke = clrBtnOutlineSelected,
                                        Fill = clrBtnBgSelected,
                                        StrokeLineWidth = 3.0f,
                                        OnPress = (cmp, p) => {
                                            var btn = (Button)cmp;

                                            // Footswitch controls scene prev/next.
                                            footswitchMapping = FootSwitchScene(controller);
                                            if (selectedComponent is Button selectedBtn) {
                                                selectedBtn.Stroke = clrBtnOutline;
                                                selectedBtn.Fill = clrBtnBg;
                                            }
                                            btn.Stroke = clrBtnOutlineSelected;
                                            btn.Fill = clrBtnBgSelected;
                                            selectedComponent = cmp;
                                            return true;
                                        },
                                        Children = {
                                            new Label(platform)
                                            {
                                                TextFont = vera,
                                                TextColor = white,
                                                Text = () => $"SCENE:  {controller.CurrentSceneDisplay}"
                                            }
                                        }
                                    })
                                }
                            },
                            new Panel(platform) {
                                //Fill = clrBtnBg,
                                //Stroke = clrBtnOutline,
                            }
                        }
                    }
                }
            };

            // TODO: add RESET button
            // TODO: add +/- buttons for scene
            // TODO: select scene button to have footswitches control prev/next scene
            // TODO: select amp control to have footswitches control +/- value of control

            this.root.CalculateLayout();
        }

        TouchEvent touch = new TouchEvent
        {
            Point = new Point(0, 0),
            Action = TouchAction.Released
        };

        void Platform_InputEvent(InputEvent ev)
        {
            if (ev.TouchEvent.HasValue)
            {
                // Record last touch point:
                touch = ev.TouchEvent.Value;

                //Console.WriteLine($"{touch.Point.X}, {touch.Point.Y}, {touch.Action}");

                this.root.HandleAction(touch.Point, touch.Action);
            }
            else if (ev.FootSwitchEvent.HasValue)
            {
                FootSwitchEvent fsw = ev.FootSwitchEvent.Value;

                if (fsw.Action == FootSwitchAction.Pressed)
                {
                    // Start batching up MIDI updates while the footswitch is held:
                    controller.StartMidiBatch();
                }

                if (fsw.Action != FootSwitchAction.Released)
                {
                    if (fsw.FootSwitch == FootSwitch.Left)
                    {
                        footswitchMapping.Left?.Invoke();
                    }
                    else if (fsw.FootSwitch == FootSwitch.Right)
                    {
                        footswitchMapping.Right?.Invoke();
                    }
                }

                if (fsw.Action == FootSwitchAction.Released)
                {
                    // Finish the batch and send out the most recent MIDI updates:
                    controller.EndMidiBatch();
                }
            }
        }

        public void Dispose()
        {
            platform.VG.DestroyFont(vera);
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
            vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
            this.root.Render();
            //if (touch.Pressed && btnReset.IsPointInside(touch.Point))
            //{
            //    controller.ActivateSong(controller.CurrentSong, controller.CurrentScene);
            //}

            // Draw touch cursor:
            if (touch.Action != TouchAction.Released)
            {
                vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
                vg.PushMatrix();
                vg.Translate(touch.Point.X, touch.Point.Y);
                vg.FillPaint = pointColor;
                point.Render(PaintMode.VG_FILL_PATH);
                vg.PopMatrix();
            }
        }
    }
}
