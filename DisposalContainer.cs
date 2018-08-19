using System;
namespace e_sharp_minor
{
    public class DisposalContainer : IDisposable
    {
        private readonly IDisposable[] objects;

        public DisposalContainer(params IDisposable[] objects) => this.objects = objects;

        public void Dispose()
        {
            foreach (var obj in objects)
            {
                obj.Dispose();
            }
        }
    }
}
