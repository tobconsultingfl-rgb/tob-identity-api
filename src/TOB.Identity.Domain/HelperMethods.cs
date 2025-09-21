using System;

namespace TOB.Identity.Domain;

public static class HelperMethods
{
    public static State GetStateEnumValue(string stateName)
    {
        if (!Enum.TryParse(stateName, out State state))
        {
            return State.IL;
        }

        return state;
    }

    public static bool GetBoolFromYesNo(string yesNo)
    {
        if (yesNo?.Trim().ToLower() == "yes")
        {
            return true;
        }

        return false;
    }
}
