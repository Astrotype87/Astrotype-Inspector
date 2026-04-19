using UnityEngine;

namespace AstrotypeInspector.Demo
{
    public class TitleDecorationTest_SingleAttribute : MonoBehaviour
    {
        public enum Sex { Male, Female, Other }
        
        [Title("Pilot Registration", "Register for ATLAS Racing Championship 2200 Season", separator: true)]
        [Space]
        [Subtitle("Full name")]
        [SerializeField] private string firstName;
        [SerializeField] private string middleName;
        [SerializeField] private string lastName;
        
        [Space]
        [Subtitle("Bio details")]
        [SerializeField] private int age;
        [SerializeField] private Sex sex;
    }
}
