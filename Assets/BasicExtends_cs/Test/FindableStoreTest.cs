using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicExtends {
    public class FindableStoreTest: TestComponent<FindableStore> {

        public string TryGetTest () {
            return Fail();
        }

        public string TryGetTest2 () {
            return Fail();
        }

        public string GenerateTest1 () {
            return Fail();
        }

        public string GenerateTest2 () {
            // Buffer増槽
            return Fail();
        }

        public string TrySetTest1 () {
            return Fail();
        }

        public string TrySetTest2 () {
            return Fail();
        }
    }
}