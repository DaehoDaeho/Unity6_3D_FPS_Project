using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        CheckRequiredReferences();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckRequiredReferences()
    {
        if(characterController == null)
        {
            Debug.LogWarning("CharacterController가 연결되지 않았습니다.", this);
        }

        if (cameraPivot == null)
        {
            Debug.LogWarning("CameraPivot이 연결되지 않았습니다.", this);
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerCamera가 연결되지 않았습니다.", this);
        }
    }
}
