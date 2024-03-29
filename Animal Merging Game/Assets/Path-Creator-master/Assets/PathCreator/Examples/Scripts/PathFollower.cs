using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        float distanceTravelled;
        [SerializeField] private GameObject movingParticles;
        [SerializeField] private AudioClip moveAudio;
        [SerializeField] private float movePitch;
        [SerializeField] private float moveVolume;
        public AudioSource aS;
        public bool canMove = false;
        public bool canPlayAudio = true;

        void Awake()
        {
            aS = GetComponent<AudioSource>();
            canMove = false;
            canPlayAudio = true;
            movingParticles.SetActive(false);
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        }


        void Start() {
            if (pathCreator != null)
            {
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null && canMove)
            {
                PlayMoveAudio();

                distanceTravelled += speed * Time.deltaTime;
                movingParticles.SetActive(true);
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        private void PlayMoveAudio()
        {

            if (canPlayAudio)
            {
                aS.resource = moveAudio;
                aS.loop = true;
                aS.volume = moveVolume;
                aS.pitch = movePitch;
                aS.Play();
                canPlayAudio = false;
            }

        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}