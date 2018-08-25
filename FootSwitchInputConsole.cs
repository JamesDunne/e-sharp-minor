using System;
namespace e_sharp_minor
{
    public class FootSwitchInputConsole : IFootSwitchInput
    {
        public FootSwitchInputConsole()
        {
        }

        public event FootSwitchEventListener EventListener;

        public void Dispose()
        {
        }

        public void PollEvents()
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    EventListener(new FootSwitchEvent
                    {
                        FootSwitch = FootSwitch.Left,
                        WhatAction = FootSwitchAction.Pressed
                    });
                }
                if (key.Key == ConsoleKey.RightArrow)
                {
                    EventListener(new FootSwitchEvent
                    {
                        FootSwitch = FootSwitch.Right,
                        WhatAction = FootSwitchAction.Pressed
                    });
                }
            }
        }
    }
}
