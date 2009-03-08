require File.dirname(__FILE__) + '/../Ninject.Tests.dll'
include Ninject::Tests::Fakes

to_configure_ninject do |ninject|
  ninject.bind IWeapon, :to => Shuriken
  ninject.bind IWeapon, :to => Sword, :name => "sword"
end