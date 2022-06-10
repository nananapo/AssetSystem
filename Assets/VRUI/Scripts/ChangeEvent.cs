using System;

namespace VRUI
{
    public class ChangeEvent<T>
    {
        private Action<T> _actions;

        public void Invoke(T arg)
        {
            _actions?.Invoke(arg);
        }

        public void AddListener(Action<T> action)
        {
            _actions += action;
        }

        public void RemoveListener(Action<T> action)
        {
            _actions -= action;
        }

        public static ChangeEvent<T> operator +(ChangeEvent<T> ev, Action<T> action)
        {
            ev ??= new ChangeEvent<T>();
            ev.AddListener(action);
            return ev;
        }

        public static ChangeEvent<T> operator -(ChangeEvent<T> ev, Action<T> action)
        {
            ev ??= new ChangeEvent<T>();
            ev.RemoveListener(action);
            return ev;
        }
    }
    
    public class ChangeEvent
    {
        private Action _actions;

        public void Invoke()
        {
            _actions?.Invoke();
        }

        public void AddListener(Action action)
        {
            _actions += action;
        }

        public void RemoveListener(Action action)
        {
            _actions -= action;
        }

        public static ChangeEvent operator +(ChangeEvent ev, Action action)
        {
            ev ??= new ChangeEvent();
            ev.AddListener(action);
            return ev;
        }

        public static ChangeEvent operator -(ChangeEvent ev, Action action)
        {
            ev ??= new ChangeEvent();
            ev.RemoveListener(action);
            return ev;
        }
    }
}