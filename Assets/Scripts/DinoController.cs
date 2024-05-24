using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DinoController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed = 5f;
    [SerializeField] private float _boundaryLimit = 0.5f;

    private RaptorSoundEffects _soundEffects;
    private bool _isMoving = false;

    private void Start()
    {
        _soundEffects = GetComponent<RaptorSoundEffects>();
    }

    private void FixedUpdate()
    {
        // Get the joystick input values
        float horizontalInput = _joystick.Horizontal;
        float verticalInput = _joystick.Vertical;

        // Calculate the movement vector based on the joystick input
        Vector3 movement = new Vector3(-horizontalInput, 0f, -verticalInput) * _moveSpeed;

        // Apply the movement to the dinosaur's rigidbody
        _rigidbody.velocity = movement;

        // Rotate the dinosaur to face the direction of movement
        if (movement != Vector3.zero)
        {
            // Calculate the desired rotation based on the joystick input
            Quaternion targetRotation = Quaternion.Euler(0f, Mathf.Atan2(-horizontalInput, -verticalInput) * Mathf.Rad2Deg + 180f, 0f);

            // Smoothly rotate the dinosaur towards the desired rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed * Time.fixedDeltaTime);

            _animator.SetBool("isRunning", true);

            // Play movement sound effects
            if (!_isMoving)
            {
                _isMoving = true;
                StartCoroutine(PlayMovementSounds());
            }
        }
        else
        {
            _animator.SetBool("isRunning", false);
            _isMoving = false;
        }

        // Limit the dinosaur's position within the Image Target bounds
        Vector3 clampedPosition = transform.localPosition;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -_boundaryLimit, _boundaryLimit);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -_boundaryLimit, _boundaryLimit);
        transform.localPosition = clampedPosition;
    }

    private IEnumerator PlayMovementSounds()
    {
        while (_isMoving)
        {
            // Play random movement sound effects
            int randomIndex = Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0:
                    _soundEffects.Growl();
                    break;
                case 1:
                    _soundEffects.Sniff();
                    break;
                case 2:
                    _soundEffects.Yelp();
                    break;
            }

            // Wait for a random interval before playing the next sound
            float randomInterval = Random.Range(1f, 3f);
            yield return new WaitForSeconds(randomInterval);
        }
    }
}