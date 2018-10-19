using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using EMinor.UI;
using EMinor.V6;
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
        private readonly PaintColor clrBtnOutlineSelected;
        private readonly PaintColor clrBtnBg;
        private readonly PaintColor clrBtnBgSelected;
        private readonly PaintColor white;
        private readonly PaintColor pointColor;
        private readonly Ellipse point;
        private readonly VGFont vera;
        private readonly Component root;
        private readonly VerticalStack ampStack;

        private Component selectedComponent;
        private readonly AutoResetEvent needFrame;

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
        private FootSwitchMapping footswitchMapping;

        public VGUI(IPlatform platform, Controller controller)
        {
            this.controller = controller;
            this.platform = platform;
            vg = platform.VG;
            footswitchMapping = FootSwitchScene(controller);
            selectedComponent = null;

            needFrame = new AutoResetEvent(false);

            vg.ClearColor = new float[] { 0.0f, 0.0f, 0.2f, 1.0f };

            platform.InputEvent += Platform_InputEvent;

            Debug.WriteLine("Set rendering quality and pixel layout");
            //vg.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_BETTER);
            //vg.Seti(ParamType.VG_PIXEL_LAYOUT, (int)PixelLayout.VG_PIXEL_LAYOUT_RGB_HORIZONTAL);

            vg.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_FASTER);
            //vg.Seti(ParamType.VG_RENDERING_QUALITY, (int)RenderingQuality.VG_RENDERING_QUALITY_NONANTIALIASED);

            // Load TTF font:
            Debug.WriteLine("Load Vera.ttf");
            var vgRasterizer = new VGFontConverter(vg);
            vera = vgRasterizer.OpenFont("Vera.ttf");

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
                                    // MIDI button:
                                    new Button(platform) {
                                        Dock = Dock.Left,
                                        Width = 60,
                                        Stroke = clrBtnOutline,
                                        Fill = clrBtnBg,
                                        StrokePressed = clrBtnBg,
                                        FillPressed = clrBtnOutline,
                                        OnRelease = (cmp, p) => {
                                            controller.MidiResend();
                                            return true;
                                        },
                                        Children = {
                                            new Label(platform)
                                            {
                                                TextFont = vera,
                                                TextColor = white,
                                                TextHAlign = HAlign.Center,
                                                TextVAlign = VAlign.Middle,
                                                Text = () => "MIDI"
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
                                        // TODO: add +/- buttons for scene
                                    })
                                } // Children
                            }, // HorizontalStack
                            (ampStack = new VerticalStack(platform) {
                                Children = controller.LiveAmps.Select(amp => createAmpComponents(platform, amp)).ToList()
                            })
                        } // Children
                    }  // VerticalStack
                }
            };

            this.root.CalculateLayout();
        }

        private Component createAmpComponents(IPlatform platform, LiveAmp amp)
        {
            return new VerticalStack(platform)
            {
                Children = {
                    new HorizontalStack(platform) {
                        Dock = Dock.Top,
                        Height = 18,
                        Children = {
                            // Amp name label:
                            new Label(platform) {
                                Text = () => amp.AmpDefinition.Name,
                                TextFont = vera,
                                TextColor = white
                            }
                        }
                    },
                    new VerticalStack(platform) {
                        Children = {
                            // Volume slider:
                            new HorizontalStack(platform) {
                                Children = {
                                    new Label(platform) {
                                        Dock = Dock.Left,
                                        Width = 80,
                                        TextFont = vera,
                                        TextColor = white,
                                        TextSize = 16,
                                        TextVAlign = VAlign.Middle,
                                        Text = () => $"Volume"
                                    },
                                    new Label(platform) {
                                        Dock = Dock.Right,
                                        Width = 80,
                                        TextFont = vera,
                                        TextColor = white,
                                        TextSize = 16,
                                        TextVAlign = VAlign.Middle,
                                        Text = () => $"{amp.Volume,4:N1} dB"
                                    },
                                    new HSlider(platform) {
                                        Fill = clrBtnBg,
                                        Stroke = clrBtnOutline,
                                        MinValue = 0.0f,
                                        MaxValue = 127.0f,
                                        Value = () => (float)amp.VolumeMIDI,
                                        ValueChanged = (value) => {
                                            amp.VolumeMIDI = (int)value;
                                            controller.ActivateLiveAmp(amp);
                                        }
                                    }
                                }
                            },
                            // Gain slider:
                            new HorizontalStack(platform) {
                                Children = {
                                    new Label(platform) {
                                        Dock = Dock.Left,
                                        Width = 80,
                                        TextFont = vera,
                                        TextColor = white,
                                        TextSize = 16,
                                        TextVAlign = VAlign.Middle,
                                        Text = () => $"Gain"
                                    },
                                    new Label(platform) {
                                        Dock = Dock.Right,
                                        Width = 80,
                                        TextFont = vera,
                                        TextColor = white,
                                        TextSize = 16,
                                        TextVAlign = VAlign.Middle,
                                        Text = () => $"{amp.Gain:X02}"
                                    },
                                    new HSlider(platform) {
                                        Fill = clrBtnBg,
                                        Stroke = clrBtnOutline,
                                        MinValue = 0.0f,
                                        MaxValue = 127.0f,
                                        Value = () => (float)amp.Gain,
                                        ValueChanged = (value) => {
                                            amp.Gain = (int)value;
                                            controller.ActivateLiveAmp(amp);
                                        }
                                    }
                                }
                            }
                        }
                    },
                    // FX toggle buttons on bottom:
                    new HorizontalStack(platform)
                    {
                        Dock = Dock.Bottom,
                        Height = 32,
                        Children = amp.FX.Select(fx => (Component)new Button(platform)
                        {
                            Fill = clrBtnBg,
                            Stroke = clrBtnOutline,
                            Children = {
                                new Label(platform) {
                                    Text = () => fx.BlockName,
                                    TextHAlign = HAlign.Center,
                                    TextVAlign = VAlign.Middle,
                                    TextFont = vera,
                                    TextColor = white
                                }
                            }
                        }).ToList()
                    }
                }
            };
        }

        TouchEvent touch = new TouchEvent
        {
            Point = new Point(0, 0),
            Action = TouchAction.Released
        };
        private Component componentPressed;

        void RecreateAmpLayout()
        {
            // Recreate UI components after scene activation:
            ampStack.Children = controller.LiveAmps.Select(amp => createAmpComponents(platform, amp)).ToList();
        }

        void Platform_InputEvent(InputEvent ev)
        {
            if (ev.TouchEvent.HasValue)
            {
                // Record last touch point:
                lock (point)
                {
                    touch = ev.TouchEvent.Value;
                }

                //Console.WriteLine($"{touch.Point.X}, {touch.Point.Y}, {touch.Action}");

                lock (root)
                {
                    if (touch.Action == TouchAction.Pressed)
                    {
                        this.componentPressed = this.root.FindPressableComponent(touch.Point);
                        this.componentPressed?.HandleAction(touch.Point, touch.Action);
                    }
                    else
                    {
                        this.componentPressed?.HandleAction(touch.Point, touch.Action);

                        if (touch.Action == TouchAction.Released)
                        {
                            this.componentPressed = null;
                        }
                    }
                }
            }
            else if (ev.FootSwitchEvent.HasValue)
            {
                FootSwitchEvent fsw = ev.FootSwitchEvent.Value;

                lock (root)
                {
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

                        RecreateAmpLayout();
                    }

                    if (fsw.Action == FootSwitchAction.Released)
                    {
                        // Finish the batch and send out the most recent MIDI updates:
                        controller.EndMidiBatch();
                    }
                }
            }

            needFrame.Set();
        }

        public void Dispose()
        {
            platform.VG.DestroyFont(vera);
            this.disposalContainer.Dispose();
        }

        public void Render()
        {
            vg.Clear(0, 0, platform.FramebufferWidth, platform.FramebufferHeight);

            lock (root)
            {
                vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
                root.Render();
            }

            // Draw touch cursor:
            if (touch.Action != TouchAction.Released)
            {
                lock (point)
                {
                    vg.Seti(ParamType.VG_MATRIX_MODE, (int)MatrixMode.VG_MATRIX_PATH_USER_TO_SURFACE);
                    vg.PushMatrix();
                    vg.Translate(touch.Point.X, touch.Point.Y);
                    vg.FillPaint = pointColor;
                    point.Render(PaintMode.VG_FILL_PATH);
                    vg.PopMatrix();
                }
            }

            // Swap buffers to display and vsync (if applicable):
            platform.SwapBuffers();
        }

        public void EndFrame()
        {
            needFrame.WaitOne();
            needFrame.Reset();
        }
    }
}
