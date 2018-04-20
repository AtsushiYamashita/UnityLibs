using System.Collections;
using System.Collections.Generic;
using BasicExtends;
using System.Linq;
using System;

public class CoroutinTest :  TestComponent{

	public string CountUpTest1 () {
        var sum = CountUp(0, 100).Sum();
        if(sum != (0 + 99) * 50) {
            return Fail(""+sum);
        }
        return Pass();
    }

    public string CountUpTest2 () {
        var sum = CountUp(0, 100,(i)=> { return i+1; }).Sum();
        if (sum != (0 + 99) * 50) {
            return Fail("" + sum);
        }
        return Pass();
    }

    IEnumerable<int> CountUp(int a, int b) {
        for(int i = a; i < b; i++) {
            yield return i;
        }
    }

    IEnumerable<int> CountUp ( int a, int b ,Func<int,int> func) {
        for (int i = a; i < b; i = func(i)) {
            yield return i;
        }
    }
}
