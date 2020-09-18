namespace TheCursedBroom.Player {
    public class BroomData {
        public bool canDash {
            get => m_canDash;
            set {
                if (m_canDash != value) {
                    m_canDash = value;
                    isDirty = true;
                }
            }
        }
        bool m_canDash;

        public bool isAligned {
            get => m_isAligned;
            set {
                if (m_isAligned != value) {
                    m_isAligned = value;
                    isDirty = true;
                }
            }
        }
        bool m_isAligned;

        public bool isDiving {
            get => m_isDiving;
            set {
                if (m_isDiving != value) {
                    m_isDiving = value;
                    isDirty = true;
                }
            }
        }
        bool m_isDiving;

        public bool isDashing {
            get => m_isDashing;
            set {
                if (m_isDashing != value) {
                    m_isDashing = value;
                    isDirty = true;
                }
            }
        }
        bool m_isDashing;

        public bool isFlying {
            get => m_isFlying;
            set {
                if (m_isFlying != value) {
                    m_isFlying = value;
                    isDirty = true;
                }
            }
        }
        bool m_isFlying;

        bool isDirty;
        public BroomState state {
            get {
                if (isDirty) {
                    isDirty = false;
                    m_state = CalculateState();
                }
                return m_state;
            }
        }
        BroomState m_state;

        BroomState CalculateState() {
            if (isFlying) {
                if (isDashing) {
                    return BroomState.Dashing;
                }
                if (canDash) {
                    return BroomState.Gliding;
                }
                if (isAligned) {
                    return isDiving
                        ? BroomState.Diving
                        : BroomState.Soaring;
                }
                return BroomState.Plummeting;
            }
            return canDash
                ? BroomState.Idling
                : BroomState.Falling;
        }
    }
}