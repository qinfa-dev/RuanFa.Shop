using ErrorOr;
using RuanFa.FashionShop.SharedKernel.Domains.Entities;
using RuanFa.FashionShop.SharedKernel.Interfaces.Domain;

namespace RuanFa.FashionShop.SharedKernel.Tests.Domains;

public class EntityTests
{
    public sealed class TestEvent : IDomainEvent { }
    public sealed class TestEntity : Entity<Guid>
    {
        public TestEntity() : base() { }
        public TestEntity(Guid id) : base(id) { }
        // Helper methods for testing - exposing the results directly
        public void AddTestEvent(IDomainEvent domainEvent) => AddDomainEvent(domainEvent);
        public void ClearTestEvents() => ClearDomainEvents();
    }
    private class DifferentTestEntity(Guid id) : Entity<Guid>(id)
    {
    }

    #region Test Case Records
    public record IdTestCase(Guid Id, string Description);
    public record DomainEventTestCase(int EventCount, string Description);
    public record EqualityTestCase(Guid Id1, Guid Id2, bool Expected, string Description);
    public record ValidationTestCase(Guid Id, bool ShouldSucceed, string ExpectedError, string Description);
    public record BatchCreationTestCase(Guid[] Ids, bool ShouldSucceed, string Description);
    public record HashCodeTestCase(Guid Id1, Guid Id2, bool ShouldBeEqual, string Description);
    #endregion

    #region Test Helper Methods
    private static Guid NewGuid() => Guid.NewGuid();
    private static Guid EmptyGuid() => Guid.Empty;
    private static Guid SpecificGuid1() => new("12345678-1234-1234-1234-123456789012");
    private static Guid SpecificGuid2() => new("87654321-4321-4321-4321-210987654321");
    private static Guid SpecificGuid3() => new("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE");
    #endregion

    #region Constructor Tests
    public class ConstructorTests
    {
        [Fact]
        public void DefaultConstructor_InitializesEmptyDomainEvents()
        {
            var entity = new TestEntity();
            entity.DomainEvents.ShouldNotBeNull();
            entity.DomainEvents.Count.ShouldBe(0);
        }

        public static TheoryData<IdTestCase> IdConstructorTestCases => new()
        {
            new(NewGuid(), "Random GUID"),
            new(SpecificGuid1(), "Specific GUID pattern 1"),
            new(SpecificGuid2(), "Specific GUID pattern 2"),
            new(EmptyGuid(), "Empty GUID"),
            new(new Guid("00000000-0000-0000-0000-000000000001"), "Minimal non-empty GUID"),
            new(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), "Maximum GUID value"),
            new(Guid.Parse("12345678-90AB-CDEF-1234-567890ABCDEF"), "Mixed case GUID")
        };

        [Theory]
        [MemberData(nameof(IdConstructorTestCases))]
        public void IdConstructor_SetsId(IdTestCase testCase)
        {
            var entity = new TestEntity(testCase.Id);
            entity.Id.ShouldBe(testCase.Id);
        }
    }
    #endregion

    #region Domain Event Tests
    public class DomainEventTests
    {
        public static TheoryData<DomainEventTestCase> MultipleEventsTestCases => new()
        {
            new(1, "Single event"),
            new(3, "Multiple events"),
            new(5, "Several events"),
            new(10, "Many events"),
            new(0, "No events")
        };

        [Theory]
        [MemberData(nameof(MultipleEventsTestCases))]
        public void AddDomainEvent_WithMultipleEvents_AddsCorrectCount(DomainEventTestCase testCase)
        {
            var entity = new TestEntity();

            for (int i = 0; i < testCase.EventCount; i++)
            {
                entity.AddTestEvent(new TestEvent());
            }

            entity.DomainEvents.Count.ShouldBe(testCase.EventCount);
        }

        [Fact]
        public void AddDomainEvent_WithValidEvent_AddsToList()
        {
            var entity = new TestEntity();
            var domainEvent = new TestEvent();
            entity.AddTestEvent(domainEvent);
            entity.DomainEvents.Count.ShouldBe(1);
            entity.DomainEvents.ShouldContain(domainEvent);
        }

        public static TheoryData<DomainEventTestCase> ClearEventsTestCases => new()
        {
            new(0, "No initial events"),
            new(1, "Single initial event"),
            new(5, "Multiple initial events"),
            new(10, "Many initial events"),
            new(100, "Large number of initial events")
        };

        [Theory]
        [MemberData(nameof(ClearEventsTestCases))]
        public void ClearDomainEvents_WithVariousEventCounts_RemovesAllEvents(DomainEventTestCase testCase)
        {
            var entity = new TestEntity();

            // Add initial events
            for (int i = 0; i < testCase.EventCount; i++)
            {
                entity.AddTestEvent(new TestEvent());
            }

            entity.ClearTestEvents();
            entity.DomainEvents.Count.ShouldBe(0);
        }
    }
    #endregion

    #region Equality Tests
    public class EqualityTests
    {
        public static TheoryData<IdTestCase> SameIdTestCases => new()
        {
            new(SpecificGuid1(), "Specific GUID pattern 1"),
            new(SpecificGuid2(), "Specific GUID pattern 2"),
            new(SpecificGuid3(), "Specific GUID pattern 3"),
            new(EmptyGuid(), "Empty GUID"),
            new(new Guid("12345678-1234-1234-1234-123456789012"), "Standard format GUID"),
            new(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), "Maximum GUID value"),
            new(new Guid("00000000-0000-0000-0000-000000000001"), "Minimal non-empty GUID")
        };

        [Theory]
        [MemberData(nameof(SameIdTestCases))]
        public void Equals_SameIdAndType_ReturnsTrue(IdTestCase testCase)
        {
            var entity1 = new TestEntity(testCase.Id);
            var entity2 = new TestEntity(testCase.Id);

            entity1.Equals(entity2).ShouldBeTrue();
            (entity1 == entity2).ShouldBeTrue();
        }

        public static TheoryData<EqualityTestCase> DifferentIdTestCases => new()
        {
            new(SpecificGuid1(), SpecificGuid2(), false, "Different specific GUIDs"),
            new(SpecificGuid2(), SpecificGuid3(), false, "Different specific GUIDs 2"),
            new(EmptyGuid(), SpecificGuid1(), false, "Empty vs non-empty GUID"),
            new(NewGuid(), NewGuid(), false, "Two random GUIDs"),
            new(new Guid("12345678-1234-1234-1234-123456789012"),
                new Guid("12345678-1234-1234-1234-123456789013"), false, "Similar GUIDs with one digit difference"),
            new(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                new Guid("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"), false, "Pattern GUIDs A vs B"),
            new(new Guid("00000000-0000-0000-0000-000000000001"),
                new Guid("00000000-0000-0000-0000-000000000002"), false, "Sequential minimal GUIDs")
        };

        [Theory]
        [MemberData(nameof(DifferentIdTestCases))]
        public void Equals_DifferentId_ReturnsFalse(EqualityTestCase testCase)
        {
            var entity1 = new TestEntity(testCase.Id1);
            var entity2 = new TestEntity(testCase.Id2);

            entity1.Equals(entity2).ShouldBe(testCase.Expected);
            (entity1 != entity2).ShouldBe(!testCase.Expected);
        }

        public static TheoryData<IdTestCase> DifferentTypeTestCases => new()
        {
            new(SpecificGuid1(), "Specific GUID with different entity types"),
            new(SpecificGuid2(), "Another specific GUID with different entity types"),
            new(EmptyGuid(), "Empty GUID with different entity types"),
            new(NewGuid(), "Random GUID with different entity types"),
            new(new Guid("12345678-1234-1234-1234-123456789012"), "Standard GUID with different entity types")
        };

        [Theory]
        [MemberData(nameof(DifferentTypeTestCases))]
        public void Equals_DifferentType_ReturnsFalse(IdTestCase testCase)
        {
            var entity1 = new TestEntity(testCase.Id);
            var entity2 = new DifferentTestEntity(testCase.Id);

            entity1.Equals(entity2).ShouldBeFalse();
            (entity1 != entity2).ShouldBeTrue();
        }

        public static TheoryData<IdTestCase> NullComparisonTestCases => new()
        {
            new(SpecificGuid1(), "Specific GUID vs null"),
            new(SpecificGuid2(), "Another specific GUID vs null"),
            new(EmptyGuid(), "Empty GUID vs null"),
            new(NewGuid(), "Random GUID vs null"),
            new(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), "Maximum GUID vs null")
        };

        [Theory]
        [MemberData(nameof(NullComparisonTestCases))]
        public void Equals_Null_ReturnsFalse(IdTestCase testCase)
        {
            var entity = new TestEntity(testCase.Id);

            entity.Equals(null).ShouldBeFalse();
            (entity == null).ShouldBeFalse();
            (null != entity).ShouldBeTrue();
        }

        // Complex equality test cases using records
        public record ComplexEqualityTestCase(TestEntity Entity1, TestEntity Entity2, bool Expected, string Description);

        public static TheoryData<ComplexEqualityTestCase> ComplexEqualityTestCases => new()
        {
            new(new TestEntity(SpecificGuid1()), new TestEntity(SpecificGuid1()), true, "Same specific GUID entities should be equal"),
            new(new TestEntity(SpecificGuid2()), new TestEntity(SpecificGuid2()), true, "Same specific GUID entities should be equal 2"),
            new(new TestEntity(SpecificGuid1()), new TestEntity(SpecificGuid2()), false, "Different GUID entities should not be equal"),
            new(new TestEntity(EmptyGuid()), new TestEntity(SpecificGuid1()), false, "Empty vs non-empty GUID should not be equal"),
            new(new TestEntity(EmptyGuid()), new TestEntity(EmptyGuid()), true, "Same empty GUID entities should be equal"),
            new(new TestEntity(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")),
                new TestEntity(new Guid("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA")), true, "Same pattern GUID entities should be equal")
        };

        [Theory]
        [MemberData(nameof(ComplexEqualityTestCases))]
        public void Equals_VariousScenarios_ReturnsExpectedResult(ComplexEqualityTestCase testCase)
        {
            var result = testCase.Entity1.Equals(testCase.Entity2);
            result.ShouldBe(testCase.Expected);

            // Also test operator overloads
            (testCase.Entity1 == testCase.Entity2).ShouldBe(testCase.Expected);
            (testCase.Entity1 != testCase.Entity2).ShouldBe(!testCase.Expected);
        }

        public static TheoryData<HashCodeTestCase> HashCodeTestCases => new()
        {
            new(SpecificGuid1(), SpecificGuid1(), true, "Same specific GUID should have same hash code"),
            new(SpecificGuid2(), SpecificGuid2(), true, "Same specific GUID should have same hash code 2"),
            new(EmptyGuid(), EmptyGuid(), true, "Same empty GUID should have same hash code"),
            new(SpecificGuid1(), SpecificGuid2(), false, "Different GUIDs may have different hash codes"),
            new(EmptyGuid(), SpecificGuid1(), false, "Empty vs non-empty GUID may have different hash codes"),
            new(new Guid("12345678-1234-1234-1234-123456789012"),
                new Guid("12345678-1234-1234-1234-123456789013"), false, "Similar GUIDs may have different hash codes")
        };

        [Theory]
        [MemberData(nameof(HashCodeTestCases))]
        public void GetHashCode_ConsistencyTest(HashCodeTestCase testCase)
        {
            var entity1 = new TestEntity(testCase.Id1);
            var entity2 = new TestEntity(testCase.Id2);

            var hash1 = entity1.GetHashCode();
            var hash2 = entity2.GetHashCode();

            if (testCase.ShouldBeEqual)
            {
                hash1.ShouldBe(hash2);
            }

            // Ensure hash codes are consistent (same object returns same hash)
            entity1.GetHashCode().ShouldBe(hash1);
            entity2.GetHashCode().ShouldBe(hash2);
        }
    }
    #endregion

    #region Integration Tests with ErrorOr
    public class ErrorOrIntegrationTests
    {
        public static TheoryData<ValidationTestCase> ValidationTestCases => new()
        {
            new(SpecificGuid1(), true, "", "Valid specific GUID"),
            new(SpecificGuid2(), true, "", "Valid specific GUID 2"),
            new(NewGuid(), true, "", "Valid random GUID"),
            new(EmptyGuid(), false, "Entity ID cannot be empty", "Invalid empty GUID"),
            new(new Guid("12345678-1234-1234-1234-123456789012"), true, "", "Valid formatted GUID"),
            new(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), true, "", "Valid maximum GUID")
        };

        [Theory]
        [MemberData(nameof(ValidationTestCases))]
        public void CreateEntity_WithErrorOrPattern_ReturnsExpectedResult(ValidationTestCase testCase)
        {
            ErrorOr<TestEntity> result = CreateEntityWithValidation(testCase.Id);

            if (testCase.ShouldSucceed)
            {
                result.IsError.ShouldBeFalse();
                result.Value.Id.ShouldBe(testCase.Id);
            }
            else
            {
                result.IsError.ShouldBeTrue();
                result.FirstError.Description.ShouldBe(testCase.ExpectedError);
            }
        }

        private static ErrorOr<TestEntity> CreateEntityWithValidation(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Error.Validation("Entity.Id", "Entity ID cannot be empty");
            }

            return new TestEntity(id);
        }

        public static TheoryData<BatchCreationTestCase> BatchCreationTestCases => new()
        {
            new(new[] { SpecificGuid1(), SpecificGuid2(), SpecificGuid3() }, true, "All valid specific GUIDs"),
            new(new[] { SpecificGuid1(), EmptyGuid(), SpecificGuid3() }, false, "Contains invalid empty GUID"),
            new(new[] { EmptyGuid(), SpecificGuid2(), SpecificGuid3() }, false, "Contains invalid empty GUID at start"),
            new(new[] { NewGuid(), NewGuid(), NewGuid(), NewGuid(), NewGuid() }, true, "Multiple valid random GUIDs"),
            new(new[] {
                new Guid("11111111-1111-1111-1111-111111111111"),
                new Guid("22222222-2222-2222-2222-222222222222"),
                new Guid("33333333-3333-3333-3333-333333333333")
            }, true, "Multiple valid pattern GUIDs"),
            new(new[] { EmptyGuid(), EmptyGuid(), EmptyGuid() }, false, "All invalid empty GUIDs"),
            new(new[] { EmptyGuid() }, false, "Single invalid empty GUID"),
            new(new[] { SpecificGuid1() }, true, "Single valid specific GUID")
        };

        [Theory]
        [MemberData(nameof(BatchCreationTestCases))]
        public void CreateMultipleEntities_WithErrorOrPattern_ReturnsExpectedResult(BatchCreationTestCase testCase)
        {
            var results = new List<ErrorOr<TestEntity>>();

            foreach (var id in testCase.Ids)
            {
                results.Add(CreateEntityWithValidation(id));
            }

            var hasErrors = results.Any(r => r.IsError);
            hasErrors.ShouldBe(!testCase.ShouldSucceed);

            if (testCase.ShouldSucceed)
            {
                results.Select(r => r.Value.Id).ShouldBe(testCase.Ids);
            }
        }

        // Advanced validation scenarios
        public record AdvancedValidationTestCase(
            Guid Id,
            bool ShouldSucceed,
            string ExpectedErrorCode,
            string ExpectedErrorDescription,
            string TestScenario
        );

        public static TheoryData<AdvancedValidationTestCase> AdvancedValidationTestCases => new()
        {
            new(SpecificGuid1(), true, "", "", "Standard valid GUID case"),
            new(NewGuid(), true, "", "", "Random valid GUID case"),
            new(new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), true, "", "", "Maximum GUID value"),
            new(new Guid("00000000-0000-0000-0000-000000000001"), true, "", "", "Minimal non-empty GUID"),
            new(EmptyGuid(), false, "Entity.Id", "Entity ID cannot be empty", "Empty GUID boundary case"),
            new(new Guid("12345678-90AB-CDEF-1234-567890ABCDEF"), true, "", "", "Mixed case hexadecimal GUID")
        };

        [Theory]
        [MemberData(nameof(AdvancedValidationTestCases))]
        public void CreateEntity_AdvancedValidation_ReturnsExpectedResult(AdvancedValidationTestCase testCase)
        {
            ErrorOr<TestEntity> result = CreateEntityWithValidation(testCase.Id);

            if (testCase.ShouldSucceed)
            {
                result.IsError.ShouldBeFalse();
                result.Value.Id.ShouldBe(testCase.Id);
            }
            else
            {
                result.IsError.ShouldBeTrue();
                result.FirstError.Code.ShouldBe(testCase.ExpectedErrorCode);
                result.FirstError.Description.ShouldBe(testCase.ExpectedErrorDescription);
            }
        }
    }
    #endregion
}
