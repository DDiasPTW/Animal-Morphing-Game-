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

        public bool canMove = false;
        //[SerializeField] private float timeTillMove = 0.5f; //time in seconds before it starts chasing the path

        void Awake()
        {
            canMove = false;
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
                distanceTravelled += speed * Time.deltaTime;
                movingParticles.SetActive(true);
                transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}