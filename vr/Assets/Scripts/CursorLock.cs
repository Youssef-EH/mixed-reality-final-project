using UnityEngine;

public class CursorLock : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode unlockKey = KeyCode.Backspace;
    public KeyCode lockKey   = KeyCode.Return;

    private bool _isLocked = true;

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(unlockKey))
        {
            UnlockCursor();
        }

        if (Input.GetKeyDown(lockKey))
        {
            LockCursor();
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isLocked = true;
        Debug.Log("CursorLock: LOCKED");
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isLocked = false;
        Debug.Log("CursorLock: UNLOCKED");
    }
}