using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TspShared;

public interface ITspSolver
{
 /**
     * Stops the calculations
     */
    public TspResults Stop();

    /**
     * Pauses the calculations
     */
    public TspResults Pause();

    /**
     * Unpauses the calculations
     */
    public void Unpause();
}