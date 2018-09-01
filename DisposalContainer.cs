using System;
using System.Collections.Generic;

namespace e_sharp_minor
{
    public class DisposalContainer : IDisposable
    {
        private readonly List<IDisposable> objects;

        public DisposalContainer(params IDisposable[] objects) => this.objects = new List<IDisposable>(objects);

        public T Add<T>(T disposable) where T : IDisposable
        {
            objects.Add(disposable);
            return disposable;
        }

        public void Dispose()
        {
            foreach (var obj in objects)
            {
                obj.Dispose();
            }
        }
    }
}
