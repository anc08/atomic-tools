/*
 * AtomicTools::ATEnums
 * Author: Adam Cohen
 * https://adamncohen08.wixsite.com/adam-cohen-game-dev
 * Extra data used in the ATStateMachine modular state machine system.
 */
namespace AtomicTools
{
    // What type of event causes a state transition to trigger
    public enum TransitionType
    {
        TriggerEnterTag = 0,
        CollisionTag = 1,
        Timer = 2,
        Hook = 3
    };

    // Types of transition conditions
    public enum ConditionType
    {
        CHOOSE,
        TagInTrigger,
        TagInCollision,
        CustomComparison
    };

    // The rule under which to evaluate whether a transition can succeed
    public enum TransitionConditionEvaluation
    {
        AnyTrue,
        AllTrue,
        AnyFalse,
        AllFalse
    };
}