namespace Sargon.Input {
    static public class Extensions {

        static public Key KeyState(this Keys k) => GameContext.Current.Input.GetKey(k);
        static public bool IsPressed(this Keys k) => KeyState(k).IsPressed;
        static public bool IsHeld(this Keys k) => KeyState(k).IsHeld;
        static public bool IsRaised(this Keys k) => KeyState(k).IsRaised;


        //static public bool IsPressed(this Keys k)

    }
}
