using System;

namespace SimulationModel.Model
{
    public static class StatName
    {
        public static String CreatedItems => "Created items";

        public static String Working => "Working";
        public static String QueueSize => "Queue size";
        public static String Failures => "Failures";
        public static String Processed => "Processed items";
        public static String AverageQueueSize => "Average queue size";
        public static String FailureProbability => "Failure probability";
        public static String AverageWorkload => "Average workload";

        public static String FinishedItems => "Finished items";
        public static String AverageTimeComplite => "Average time complite work";
        public static String AverageTimeAwait => "Average time await";
    }
}
