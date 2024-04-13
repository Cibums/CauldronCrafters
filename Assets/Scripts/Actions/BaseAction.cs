using System;

[Serializable]
public class BaseAction
{
    public virtual void InvokeAction()
    {
        //Animations and cool effects that will happen for each item
    }

    public virtual string ActionText()
    {
        return "Unknown action o_O";
    }
}
