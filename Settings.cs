using System;
using System.Collections.Generic;

namespace AnimatedArmouryRebalancer
{
    public class Settings
    {
        public bool IncludeWACCF { get; set; } = false;
        public List<string> IncludedPlugins { get; set; } = new List<string> { "NewArmoury.esp" };
    }
} 