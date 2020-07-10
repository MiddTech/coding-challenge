- The Readme suggests that when both colour and size are specified that the search results must match on *both* variables in the resulting combinations.  
It isn't clearly defined that the alternative approach of additionally matching on *either* variable from the resulting combinations is not also required.  
I've left it out of the implementation but am noting it here as doing so is possibly an incorrect interpretation of the requirement.

- The assertions for the size/colour counts in the tests seem at odds with the readme, i've altered the assertion code and commented the old assertion code out, but again, perhaps it's a misunderstanding on my part.  

- Duplicate search terms are not covered in the readme, but i assume they are to be discarded.

```NOTE: In the performance tests there is an assertion "Should().BeEquivalentTo()" , this is technically the recommended way of comparing the results, but with the large comparison sets it takes a long time, i'm comparing the counts instead as a quicker (but more imprecise) alternative.```