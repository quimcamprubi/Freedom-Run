using UnityEngine;

namespace CollectibleItems {
    public class RegularItem : CollectibleItem {
        public override void OnTriggerEnter2D(Collider2D other) {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null) {
                controller.AvailableCollectibleItem(this, gameObject);
            }
        }

        public override void OnTriggerExit2D(Collider2D other) {
            PlayerController controller = other.GetComponent<PlayerController>();
            if (controller != null) {
                controller.NoAvailableCollectibleItem();
            }
        }
    }
}
