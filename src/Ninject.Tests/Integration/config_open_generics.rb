require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes
include Ninject::Tests::Integration::StandardKernelTests

# IGeneric is a generic interface and GenericService is a generic type
# we don't have to specify any special notation for open generics

to_configure_ninject do |ninject|
  ninject.bind IGeneric, :to => GenericService
  ninject.bind IGeneric, :to => GenericService2
end