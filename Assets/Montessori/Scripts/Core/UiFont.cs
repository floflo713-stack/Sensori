using UnityEngine;

namespace Sensori.Montessori
{
    public static class UiFont
    {
        static Font _builtin;

        public static Font Builtin
        {
            get
            {
                if (_builtin == null)
                    _builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                return _builtin;
            }
        }
    }
}
