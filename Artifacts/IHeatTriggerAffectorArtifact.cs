using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheJazMaster.Nibbs.Artifacts;
public interface IHeatTriggerAffectorArtifact
{
    public int ModifyHeatTriggerTooltip(G g, Ship ship);
}