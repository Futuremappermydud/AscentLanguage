
Running the following expressions over 1000 iterations on commit `8d624c1` the following numbers are given.

Test 1 (40 tokens)
```
function test(add1, add2) {
    return add1 + add2
};

function k(a) {
    return a * 2
};

return k(test(test(2, 1), 2)) ^ 3
```
Average time: 0.03ms

Test 2 (151 tokens)
```
function test(add1, add2) {
    return add1 + add2
};

function test2(add1, add2) {
    return add1 + add2
};

function test3(add1, add2) {
    return add1 + add2
};

function test4(add1, add2) {
    return add1 + add2
};

function test5(add1, add2) {
    return add1 + add2
};

return test(test2(test3(test4(test5(2, 2), test5(2, 2)), test4(test5(2, 2), test5(2, 2)))), test2(test3(test4(test5(2, 2), test5(2, 2)), test4(test5(2, 2), test5(2, 2))))) ^ 3 * 2
```
Average Time: 0.045ms