require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes
include Ninject::Tests::Integration::StandardKernelTests

to_configure_ninject do |ninject|
  ninject.bind IGeneric, :to => GenericService
  ninject.bind IGeneric, :to => GenericService2
end