using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;
namespace Scripts.Utils
{
    public class MouseClick : MonoBehaviour
    {
        private LuaFunction m_mouseDownClick;
        private GameObject m_mouseDownGameObject = null;
        private LuaTable m_mouseDownLuaTable = null;

        private LuaFunction m_mouseUpClick;
        private GameObject m_mouseUpGameObject = null;
        private LuaTable m_mouseUpLuaTable = null;

        private LuaFunction m_mouseUpAsButtonClick;
        private GameObject m_mouseUpAsButtonGameObject = null;
        private LuaTable m_mouseUpAsButtonLuaTable = null;

        private LuaFunction m_mouseDragClick;
        private GameObject m_mouseDragGameObject = null;
        private LuaTable m_mouseDragLuaTable = null;

        void OnMouseDrag()
        {
            if (UICamera.isOverUI)
            {
                return;
            }
            if (m_mouseDragLuaTable != null)
            {
                m_mouseDragClick.Call(m_mouseDragLuaTable, m_mouseDragGameObject);
            }
        }

        void OnMouseUpAsButton()
        {
            if (UICamera.isOverUI)
            {
                return;
            }
            if (m_mouseUpAsButtonLuaTable != null)
            {
                m_mouseUpAsButtonClick.Call(m_mouseUpAsButtonLuaTable, m_mouseUpAsButtonGameObject);
            }
        }

        void OnMouseUp()
        {
            if (UICamera.isOverUI)
            {
                return;
            }
            if (m_mouseUpLuaTable != null)
            {
                m_mouseUpClick.Call(m_mouseUpLuaTable, m_mouseUpGameObject);
            }
        }

        void OnMouseDown()
        {
            if (UICamera.isOverUI)
            {
                return;
            }
            if (m_mouseDownLuaTable != null)
            {
                m_mouseDownClick.Call(m_mouseDownLuaTable, m_mouseDownGameObject);
            }
        }


        public void AddMouseDownClick(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            m_mouseDownClick = luafunc;
            m_mouseDownGameObject = go;
            m_mouseDownLuaTable = luaTable;
        }

        public void RemoveMouseDownClick()
        {
            m_mouseDownClick = null;
            m_mouseDownGameObject = null;
            m_mouseDownLuaTable = null;
        }

        public void AddMouseUpClick(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            m_mouseUpClick = luafunc;
            m_mouseUpGameObject = go;
            m_mouseUpLuaTable = luaTable;
        }

        public void RemoveMouseUpClick()
        {
            m_mouseUpClick = null;
            m_mouseUpGameObject = null;
            m_mouseUpLuaTable = null;
        }

        public void AddMouseUpAsButtonClick(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            m_mouseUpAsButtonClick = luafunc;
            m_mouseUpAsButtonGameObject = go;
            m_mouseUpAsButtonLuaTable = luaTable;
        }

        public void RemoveMouseUpAsButtonClick()
        {
            m_mouseUpAsButtonClick = null;
            m_mouseUpAsButtonGameObject = null;
            m_mouseUpAsButtonLuaTable = null;
        }

        public void AddMouseDragClick(GameObject go, LuaTable luaTable, LuaFunction luafunc)
        {
            if (go == null || luafunc == null) return;
            m_mouseDragClick = luafunc;
            m_mouseDragGameObject = go;
            m_mouseDragLuaTable = luaTable;
        }

        public void RemoveMouseDragClick()
        {
            m_mouseDragClick = null;
            m_mouseDragGameObject = null;
            m_mouseDragLuaTable = null;
        }

    }
}
