<div class='content'>

Modelbuilders are an imperative way of adding metadata and attributes to the excelenct XAF TypesInfo system.

It's based around an <a href='https://www.martinfowler.com/bliki/FluentInterface.html' target="_blank">Fluent Interface Pattern</a> and highly inspired by <a href='https://docs.microsoft.com/en-us/ef/core/modeling/' target='_blank'>EntityFramework's ModelBuilder's</a>.

ModelBuilders are one of the many `NonVisual Components` of `Xenial` and designed around best practices and working most efficiently in a team, however there are several benefits for smaller teams and projects as well.

### Reasons to use ModelBuilders

1. Write reusable Attribute code and reduce **string magic**
1. Annotate code with attributes that you can't change directly (foreign assembly)
1. Write conditional attributes (for example with external configuration)
1. Write your own Domain Specific Language around Domain datatypes
1. Avoid misconfigured Attributes, especially when you need to combine multiple attributes
1. Reduces code duplication and help you build consistent applications
1. Code refactoring becomes a lot easier

### Reasons *not* to use ModelBuilders

1. It comes (as almost anything) with a small startup performance penalty
1. It may slightly increase code complexity

</div>
