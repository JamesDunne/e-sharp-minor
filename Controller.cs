using System;
using System.Collections.Generic;
using System.Threading;
using OpenVG;
using Shapes;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static System.IO.File;

namespace e_sharp_minor
{
    public class Controller
    {
        private readonly IOpenVG vg;
        private V5.AllPrograms programs;

        public Controller(IOpenVG vg) => this.vg = vg;

        public void Run()
        {
            var de = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tr = OpenText("all-programs-v5.yml"))
                programs = de.Deserialize<V5.AllPrograms>(tr);

            var se = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            using (var tw = CreateText("all-programs-v6.yml"))
                se.Serialize(tw, programs);

            vg.Setfv(ParamType.VG_CLEAR_COLOR, new float[] { 0.0f, 0.0f, 0.2f, 1.0f });

            PaintColor strokePaint;
            PaintColor fillPaint;
            RoundRect rect;

            using (new DisposalContainer(
                strokePaint = new PaintColor(vg, new float[] { 1.0f, 1.0f, 1.0f, 1.0f }),
                fillPaint = new PaintColor(vg, new float[] { 0.6f, 0.6f, 0.6f, 1.0f }),
                rect = new RoundRect(vg, 100, 100, vg.Width - 100 * 2, vg.Height - 100 * 2, 16, 16)
                {
                    StrokeLineWidth = 1.0f
                    // vgSeti(VG_STROKE_CAP_STYLE, ps->m_paths[i].m_capStyle);
                    // vgSeti(VG_STROKE_JOIN_STYLE, ps->m_paths[i].m_joinStyle);
                    // vgSetf(VG_STROKE_MITER_LIMIT, ps->m_paths[i].m_miterLimit);
                }
            ))
            {
#if TIMING
                // Render at 60fps for 5 seconds:
                var sw = new Stopwatch();
#endif
                for (int f = 0; f < 60 * 5; f++)
                {
#if TIMING
                    sw.Restart();
#endif
                    // Render our pre-made paths each frame:
                    vg.Clear(0, 0, vg.Width, vg.Height);

                    strokePaint.Activate(PaintMode.VG_STROKE_PATH);
                    fillPaint.Activate(PaintMode.VG_FILL_PATH);
                    rect.Render(PaintMode.VG_FILL_PATH | PaintMode.VG_STROKE_PATH);

                    // Swap buffers to display and vsync:
                    vg.SwapBuffers();

#if TIMING
                    // usually writes "16 ms"
                    Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
#endif
                }
            }

            Console.WriteLine("Wait");

            Thread.Sleep(5000);

            Console.WriteLine("Shutdown");
        }
    }

    namespace V5
    {
        public class AllPrograms
        {
            public List<AmpDefault> Amp { get; set; }
            public List<Program> Programs { get; set; }
        }

        public class AmpDefault
        {
            [YamlMember(Alias = "fx_layout")]
            public List<string> FXLayout { get; set; } // `yaml:"fx_layout"`
            public int Gain { get; set; }          // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; }       // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
        }

        public class Program
        {
            public string Name { get; set; } // `yaml:"name"`
            [YamlMember(Alias = "midi", ApplyNamingConventions = false)]
            public int MidiProgram { get; set; } // `yaml:"midi"`
            public int Tempo { get; set; } // `yaml:"tempo"`
            public int Gain { get; set; } // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; } // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
            public List<AmpDefault> Amp { get; set; } // `yaml:"amp"`
            [YamlMember(Alias = "scenes", ApplyNamingConventions = false)]
            public List<SceneDescriptor> SceneDescriptors { get; set; } // `yaml:"scenes"`
        }

        public class SceneDescriptor
        {
            public string Name { get; set; }
            [YamlMember(Alias = "MG", ApplyNamingConventions = false)]
            public Amp MG { get; set; } // `yaml:"MG"`
            [YamlMember(Alias = "JD", ApplyNamingConventions = false)]
            public Amp JD { get; set; } // `yaml:"JD"`
        }

        public class Amp
        {
            public int Gain { get; set; } // `yaml:"gain"`     // amp gain (1-127), 0 means default
            public int GainLog { get; set; } // `yaml:"gain_log"` // amp gain (1-127) in log scale, 0 means default
            public string Channel { get; set; } // `yaml:"channel"`  // "clean" or "dirty"
            public double Level { get; set; } // `yaml:"level"`    // pre-delay volume in dB (-inf to +6dB)
            [YamlMember(Alias = "fx", ApplyNamingConventions = false)]
            public List<string> FX { get; set; } // `yaml:"fx,flow"`  // any combo of "delay", "pitch", or "chorus"
        }
    }
}
