using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencySpender.Helpers
{
    internal class ScheduleHelper
    {
        public static async Task WaitForConditionAsync(Func<bool> condition, int timeout = 10000, int pollingInterval = 100)
        {
            int elapsed = 0;

            while (!condition() && elapsed < timeout)
            {
                await Task.Delay(pollingInterval);
                elapsed += pollingInterval;
            }

            if (!condition())
            {
                throw new TimeoutException("Condition was not met in time.");
            }
        }
        public unsafe void WaitForDependencies()
        {
            P.TaskManager.Enqueue(() => PlayerState.Instance != null && AgentFateProgress.Instance != null);
        }

    }
}
