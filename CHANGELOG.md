# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed
- Call `kernel.Get<T>()` two times do not give the same result [#262](https://github.com/ninject/Ninject/issues/262)

## [3.3.4] - 2017-11-13

### Fixed
- Throw cyclic dependency exception when resolve a named binding with decoration pattern [#261](https://github.com/ninject/Ninject/issues/261)

## [3.3.3] - 2017-10-22

### Fixed
- Removed debug code.

## [3.3.2] - 2017-10-22

### Added
- Ninject for .NET Core can now load extensions automatically.
- Added back CLSCompliant(true)

## [3.3.2-rc1] - 2017-10-15

### Added
- Added back CLSCompliant(true)

### Removed
- Removed executing assembly's directory from the base directories.

## [3.3.2-beta1] - 2017-10-07

### Added
- Ninject for .NET Core can now load extensions automatically.
- The executing assembly's directory is considered as one of the base directories.

## [3.3.1] - 2017-10-05

### Added
- Support `kernel.Get<IEnumerable<IFoo>>()` [#252](https://github.com/ninject/Ninject/issues/252)

### Changed
- Moved BindingPrecedenceComparer to Bindings folder.

### Deprecated 
- The `GetValues` and `GetValue` methods of `Target` are obsolete.

### Fixed
- Cyclical dependency check throws false positive for decorator pattern [#251](https://github.com/ninject/Ninject/issue/251)

## [3.3.0] - 2017-09-26

### Changed
- Renamed ReleaseNotes.md to CHANGELOG.md and updated the format.

## [3.3.0-beta1] - 2017-09-23

### Added
- Support .NET Standard 2.0
- Strongly typed overloads of `WithConstructorArgument` which use a callback to get the value [#197](https://github.com/ninject/Ninject/pull/197)
- Do not choose constructors with an `ObsoleteAttribute` [#224](https://github.com/ninject/Ninject/pull/224)
- Meaningful exception message if there is error in configuration [#240](https://github.com/ninject/Ninject/issues/240) [#245](https://github.com/ninject/Ninject/issues/245)

### Changed
- Using `HasDefaultValue` instead of `DBNull` [#235](https://github.com/ninject/Ninject/issues/235)
- Array/List of concrete classes will return empty if the concrete class is not explicitly binded [#227](https://github.com/ninject/Ninject/issues/227)

### Removed
- Support for .NET 3.5 and Silverlight

### Fixed
- Improved cyclical dependencies detection [#143](https://github.com/ninject/Ninject/issues/143)
- `InvalidProgramException` when select constructors for `MulticastDelegate` [#175](https://github.com/ninject/ninject/issues/175)
- `WhenMemberHas` broken [#189](https://github.com/ninject/Ninject/issues/189)
- Injection into private parent parent properties fails [#214](https://github.com/ninject/Ninject/issues/241) [#217](https://github.com/ninject/Ninject/issues/217)
- Break Singleton / circular dependency `WithPropertyValue` or `OnActivation` callback [#221](https://github.com/ninject/Ninject/issues/221) [#224](https://github.com/ninject/Ninject/issues/224)
- The invoked member is not supported in a dynamic assembly [#225](https://github.com/ninject/Ninject/issues/225)
- Conditional binding is not being considered when score constructors [#237](https://github.com/ninject/Ninject/issues/237)

## [3.2]

### Added
- bool IRequest.ForceUnique: In case there is an uncoditional and a conditional binding, return the conditional one. In case there are multiple unconditional or conditional bindings, throw an exception.
- IResolutionRoot.TryGetAndThrowOnInvalidBinding<T> (extension method): Returns null if there is no binding, but throws ActivationException in case there is a binding which could not be activated.
- TypeMatchingConstructorArgument introduced.
- ToConstructor() can now accept results from methods as argument e.g. ToConstructor(_ => new Foo(this.GetBar())
- WhenNoAncestorMatches, WhenAnyAncestorMatches and WhenNoAncestorNamed When overloads
- WeakConstructorArgument and WeakPropertyValue that keep a weak reference to the value only so that Ninject has no reference on them when caching the created instance.
- Overloads for WhenInjectedInto and WhenInjectedExactlyInto that take multiple types to support multiple allowed parents.

### Changed
- Added WhenAnyAncestorNamed and marked mispelled WhenAnyAnchestorNamed as obsolete 
- Release method was moved from IKernel to the IResolutionRoot interface 

### Fixed
- Private properties of base class were not checked for existence of setter and Inject attribute
- When an object that is the scope of another object is released an Exception was thrown. 

## [3.0.1]

### Added
- The default scope can be changed in the NinjectSettings using

### Changed
- Open generics can now be passed to WhenInjectedInto

### Fixed
- Fixed race condition in the GarbageCollectionCachePruner

## [3.0.0]

### Changed
- The constructor scorer ignores implicit bindings
- The constructor scorer ignores self bindings

### [3.0.0-rc3]

### Added
- Support for default parameters. If not explicit binding exists for a dependency but there is default value defined it is used instead.
- Support to define the constructor and constructor arguments using ToConstructor "to" overload
- WhenInjectedExactlyInto When overload: Matches only if the target is exactly the specified type. This was the behavior of WhenInjectedInto in Ninject 2.2.
- WhenAnyAnchestorNamed. Matches if any of the anchestor bindings is named with the specified name.
- Default binding for IResolutionRoot that returns the kernel.
- Open generic bindings can be overriden by closed generics for specific types.
- Support for extensions that they can define bindings that return the same instance for different interfaces (interface segregation principle).
- Generic Overloads for OnActivation and OnDeactivation that can be used to cast the implementation type. 
- Bind<T1,T2, ...>() to define multiple interfaces for one service.
- Rebind<T1,T2, ...>() to define multiple interfaces for one service.
- Support to inject constructor arguments to deeper levels using new ConstructorArgument("name", value, true)

### Changed
- WhenInjectedInto matches also if the target derives from the specified type.
- ToConstant bindings are in singleton scope by default
- Separate project for medium trust environments.
- Open generic bindings can be overwritten by closed generic bindings
- Ninject modules have a new method VerifyRequiredModulesAreLoaded to check if their dependencies are loaded.
- If several constructors have the same score an ActivationExcpetion is thrown.

### Removed
- No web builds. All builds are have no reference to web anymore

### Fixed
- Breaking change: Get all will now return all bindings and not skip unconditional ones anymore in case there is a conditional one. This is the same behavior as the version 2.0.1 and bevore had. 
- Fixed that the CF and SL version of the activation cache did not properly remove the weak references
- (for CF): The CF version threw an exception when a class had a generic method on a base class. This bugfix has the side effect that the Inject attribute cannot be defined on base methods anymore. It has to be defined on the overriden method! 
- The constructor scorer accepts default values
- The constructor scorer accepts self bindings


## [2.2.1.0]

### Fixed
- For classes that have several virtual indexers and at least one of them overridden an ambiguous match exception was thrown when they were injected.
