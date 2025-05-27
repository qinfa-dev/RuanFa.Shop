using RuanFa.FashionShop.SharedKernel.Models;

namespace RuanFa.FashionShop.SharedKernel.Tests.Models;

public class PaginatedListTests
{
    // Test data models
    public sealed record TestItem(int Id, string Name);
    public sealed record TestItemDto(int Id, string Name, string DisplayName);

    #region Test Case Records
    public record ConstructorTestCase(IReadOnlyCollection<TestItem> Items, int TotalCount, int PageIndex, int PageSize, bool ShouldSucceed, string ExpectedError, string Description);
    public record PaginationCalculationTestCase(int TotalCount, int PageIndex, int PageSize, int ExpectedTotalPages, bool ExpectedHasPrevious, bool ExpectedHasNext, string Description);
    public record MappingTestCase(IReadOnlyCollection<TestItem> Items, int TotalCount, int PageIndex, int PageSize, string Description);
    public record EdgeCaseTestCase(int TotalCount, int PageIndex, int PageSize, string Description);
    public record ValidationTestCase(IReadOnlyCollection<TestItem> Items, int TotalCount, int PageIndex, int PageSize, bool ShouldSucceed, string ExpectedError, string Description);
    #endregion

    #region Test Helper Methods
    private static IReadOnlyCollection<TestItem> CreateTestItems(int count) =>
        Enumerable.Range(1, count).Select(i => new TestItem(i, $"Item {i}")).ToList().AsReadOnly();

    private static IReadOnlyCollection<TestItem> EmptyTestItems() => new List<TestItem>().AsReadOnly();

    private static IReadOnlyCollection<TestItem> SingleTestItem() =>
        new List<TestItem> { new(1, "Single Item") }.AsReadOnly();

    private static IReadOnlyCollection<TestItem> MultipleTestItems() =>
        new List<TestItem>
        {
            new(1, "First Item"),
            new(2, "Second Item"),
            new(3, "Third Item")
        }.AsReadOnly();

    private static TestItemDto MapToDto(TestItem item) => new(item.Id, item.Name, $"Display: {item.Name}");
    private static TestItemDto MapToDtoWithIndex(TestItem item, int index) => new(item.Id, item.Name, $"#{index + 1}: {item.Name}");
    #endregion

    #region Constructor Tests
    public class ConstructorTests
    {
        public static TheoryData<ConstructorTestCase> ValidConstructorTestCases => new()
        {
            new(EmptyTestItems(), 0, 1, 10, true, "", "Empty items with zero total count"),
            new(SingleTestItem(), 1, 1, 10, true, "", "Single item first page"),
            new(MultipleTestItems(), 3, 1, 10, true, "", "Multiple items first page"),
            new(CreateTestItems(10), 100, 5, 10, true, "", "Full page in middle of pagination"),
            new(CreateTestItems(5), 100, 10, 10, true, "", "Partial page at end"),
            new(CreateTestItems(1), 1, 1, 1, true, "", "Single item single page size"),
            new(CreateTestItems(20), 20, 1, 20, true, "", "All items on single page"),
            new(EmptyTestItems(), 100, 5, 10, true, "", "Empty page in middle of pagination"),
            new(CreateTestItems(3), 1000, 1, 3, true, "", "Small page size with large total"),
            new(CreateTestItems(100), 100, 1, 100, true, "", "Large page size matching total")
        };

        [Theory]
        [MemberData(nameof(ValidConstructorTestCases))]
        public void Constructor_WithValidParameters_CreatesInstance(ConstructorTestCase testCase)
        {
            var paginatedList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            paginatedList.Items.ShouldBe(testCase.Items);
            paginatedList.TotalCount.ShouldBe(testCase.TotalCount);
            paginatedList.PageIndex.ShouldBe(testCase.PageIndex);
            paginatedList.PageSize.ShouldBe(testCase.PageSize);
        }

        public static TheoryData<ValidationTestCase> InvalidConstructorTestCases => new()
        {
            new(null!, 10, 1, 10, false, "items", "Null items should throw ArgumentNullException"),
            new(EmptyTestItems(), -1, 1, 10, false, "totalCount", "Negative total count should throw ArgumentOutOfRangeException"),
            new(EmptyTestItems(), 10, 0, 10, false, "pageIndex", "Zero page index should throw ArgumentOutOfRangeException"),
            new(EmptyTestItems(), 10, -1, 10, false, "pageIndex", "Negative page index should throw ArgumentOutOfRangeException"),
            new(EmptyTestItems(), 10, 1, 0, false, "pageSize", "Zero page size should throw ArgumentOutOfRangeException"),
            new(EmptyTestItems(), 10, 1, -1, false, "pageSize", "Negative page size should throw ArgumentOutOfRangeException"),
            new(EmptyTestItems(), -10, -1, -5, false, "totalCount", "Multiple invalid parameters should throw first validation error"),
            new(EmptyTestItems(), int.MinValue, 1, 10, false, "totalCount", "Minimum integer total count should throw"),
            new(EmptyTestItems(), 10, int.MinValue, 10, false, "pageIndex", "Minimum integer page index should throw"),
            new(EmptyTestItems(), 10, 1, int.MinValue, false, "pageSize", "Minimum integer page size should throw")
        };

        [Theory]
        [MemberData(nameof(InvalidConstructorTestCases))]
        public void Constructor_WithInvalidParameters_ThrowsException(ValidationTestCase testCase)
        {
            var exception = Should.Throw<Exception>(() =>
                new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize));

            (exception is ArgumentNullException || exception is ArgumentOutOfRangeException)
                .ShouldBeTrue($"Expected an ArgumentNullException or ArgumentOutOfRangeException, but got {exception.GetType().Name}");
        }
    }
    #endregion

    #region Pagination Calculation Tests
    public class PaginationCalculationTests
    {
        public static TheoryData<PaginationCalculationTestCase> PaginationTestCases => new()
        {
            new(0, 1, 10, 0, false, false, "Empty collection should have no pages"),
            new(1, 1, 10, 1, false, false, "Single item should have one page"),
            new(10, 1, 10, 1, false, false, "Exact page size should have one page"),
            new(11, 1, 10, 2, false, true, "One over page size should have two pages"),
            new(100, 1, 10, 10, false, true, "First page of many"),
            new(100, 5, 10, 10, true, true, "Middle page of many"),
            new(100, 10, 10, 10, true, false, "Last page of many"),
            new(99, 10, 10, 10, true, false, "Last page with partial items"),
            new(1, 1, 1, 1, false, false, "Single item single page size"),
            new(2, 1, 1, 2, false, true, "Two items single page size first page"),
            new(2, 2, 1, 2, true, false, "Two items single page size second page"),
            new(1000, 50, 20, 50, true, false, "Large dataset last page"),
            new(999, 50, 20, 50, true, false, "Large dataset last page with partial items"),
            new(20, 3, 7, 3, true, false, "Irregular page size and count")
        };

        [Theory]
        [MemberData(nameof(PaginationTestCases))]
        public void PaginationProperties_CalculatedCorrectly(PaginationCalculationTestCase testCase)
        {
            var items = CreateTestItems(Math.Min(testCase.PageSize, testCase.TotalCount - (testCase.PageIndex - 1) * testCase.PageSize));
            if (testCase.TotalCount == 0) items = EmptyTestItems();

            var paginatedList = new PaginatedList<TestItem>(items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            paginatedList.TotalPages.ShouldBe(testCase.ExpectedTotalPages);
            paginatedList.HasPreviousPage.ShouldBe(testCase.ExpectedHasPrevious);
            paginatedList.HasNextPage.ShouldBe(testCase.ExpectedHasNext);
        }

        public static TheoryData<EdgeCaseTestCase> EdgeCaseTestCases => new()
        {
            new(0, 1, 1, "Zero total count with minimal page parameters"),
            new(0, 1, int.MaxValue, "Zero total count with maximum page size"),
            new(int.MaxValue, 1, 1, "Maximum total count with minimal page size"),
            new(int.MaxValue, int.MaxValue, int.MaxValue, "All maximum values"),
            new(1, 1, int.MaxValue, "Single item with maximum page size"),
            new(100, 1, 1000, "Small total count with large page size")
        };

        [Theory]
        [MemberData(nameof(EdgeCaseTestCases))]
        public void PaginationCalculation_EdgeCases_HandledCorrectly(EdgeCaseTestCase testCase)
        {
            var items = testCase.TotalCount == 0 ? EmptyTestItems() : CreateTestItems(1);

            Should.NotThrow(() =>
            {
                var paginatedList = new PaginatedList<TestItem>(items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

                // Basic validation that calculated properties are reasonable
                paginatedList.TotalPages.ShouldBeGreaterThanOrEqualTo(0);
                paginatedList.HasPreviousPage.ShouldBe(testCase.PageIndex > 1);

                if (testCase.TotalCount == 0)
                {
                    paginatedList.TotalPages.ShouldBe(0);
                    paginatedList.HasNextPage.ShouldBeFalse();
                }
            });
        }
    }
    #endregion

    #region Mapping Tests
    public class MappingTests
    {
        public static TheoryData<MappingTestCase> BasicMappingTestCases => new()
        {
            new(EmptyTestItems(), 0, 1, 10, "Empty items mapping"),
            new(SingleTestItem(), 1, 1, 10, "Single item mapping"),
            new(MultipleTestItems(), 3, 1, 10, "Multiple items mapping"),
            new(CreateTestItems(10), 100, 5, 10, "Full page mapping"),
            new(CreateTestItems(5), 100, 10, 10, "Partial page mapping"),
            new(CreateTestItems(1), 1000, 999, 1, "Single item large dataset"),
            new(CreateTestItems(50), 50, 1, 50, "Large page mapping"),
            new(CreateTestItems(3), 1000000, 500000, 3, "Small page in massive dataset")
        };

        [Theory]
        [MemberData(nameof(BasicMappingTestCases))]
        public void Map_BasicMapping_PreservesPaginationMetadata(MappingTestCase testCase)
        {
            var originalList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            var mappedList = originalList.Map(MapToDto);

            // Verify pagination metadata is preserved
            mappedList.TotalCount.ShouldBe(originalList.TotalCount);
            mappedList.PageIndex.ShouldBe(originalList.PageIndex);
            mappedList.PageSize.ShouldBe(originalList.PageSize);
            mappedList.TotalPages.ShouldBe(originalList.TotalPages);
            mappedList.HasPreviousPage.ShouldBe(originalList.HasPreviousPage);
            mappedList.HasNextPage.ShouldBe(originalList.HasNextPage);

            // Verify item count matches
            mappedList.Items.Count.ShouldBe(originalList.Items.Count);
        }

        [Theory]
        [MemberData(nameof(BasicMappingTestCases))]
        public void Map_BasicMapping_TransformsItemsCorrectly(MappingTestCase testCase)
        {
            var originalList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            var mappedList = originalList.Map(MapToDto);

            var originalArray = originalList.Items.ToArray();
            var mappedArray = mappedList.Items.ToArray();

            for (int i = 0; i < originalArray.Length; i++)
            {
                var original = originalArray[i];
                var mapped = mappedArray[i];

                mapped.Id.ShouldBe(original.Id);
                mapped.Name.ShouldBe(original.Name);
                mapped.DisplayName.ShouldBe($"Display: {original.Name}");
            }
        }

        [Theory]
        [MemberData(nameof(BasicMappingTestCases))]
        public void Map_WithIndex_PreservesPaginationMetadata(MappingTestCase testCase)
        {
            var originalList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            var mappedList = originalList.Map(MapToDtoWithIndex);

            // Verify pagination metadata is preserved
            mappedList.TotalCount.ShouldBe(originalList.TotalCount);
            mappedList.PageIndex.ShouldBe(originalList.PageIndex);
            mappedList.PageSize.ShouldBe(originalList.PageSize);
            mappedList.TotalPages.ShouldBe(originalList.TotalPages);
            mappedList.HasPreviousPage.ShouldBe(originalList.HasPreviousPage);
            mappedList.HasNextPage.ShouldBe(originalList.HasNextPage);
        }

        [Theory]
        [MemberData(nameof(BasicMappingTestCases))]
        public void Map_WithIndex_TransformsItemsWithIndexCorrectly(MappingTestCase testCase)
        {
            var originalList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            var mappedList = originalList.Map(MapToDtoWithIndex);

            var originalArray = originalList.Items.ToArray();
            var mappedArray = mappedList.Items.ToArray();

            for (int i = 0; i < originalArray.Length; i++)
            {
                var original = originalArray[i];
                var mapped = mappedArray[i];

                mapped.Id.ShouldBe(original.Id);
                mapped.Name.ShouldBe(original.Name);
                mapped.DisplayName.ShouldBe($"#{i + 1}: {original.Name}");
            }
        }

        [Fact]
        public void Map_WithNullMapper_ThrowsArgumentNullException()
        {
            var paginatedList = new PaginatedList<TestItem>(SingleTestItem(), 1, 1, 10);

            Should.Throw<ArgumentNullException>(() => paginatedList.Map<TestItemDto>((Func<TestItem, TestItemDto>)null!));
        }

        [Fact]
        public void Map_WithIndexAndNullMapper_ThrowsArgumentNullException()
        {
            var paginatedList = new PaginatedList<TestItem>(SingleTestItem(), 1, 1, 10);

            Should.Throw<ArgumentNullException>(() => paginatedList.Map<TestItemDto>((Func<TestItem, int, TestItemDto>)null!));
        }

        // Complex mapping scenarios
        public record ComplexMappingTestCase(
            IReadOnlyCollection<TestItem> Items,
            int TotalCount,
            int PageIndex,
            int PageSize,
            Func<TestItem, string> Mapper,
            string[] ExpectedResults,
            string Description
        );

        public static TheoryData<ComplexMappingTestCase> ComplexMappingTestCases => new()
        {
            new(
                new List<TestItem> { new(1, "Test"), new(2, "Item") }.AsReadOnly(),
                2, 1, 10,
                item => item.Name.ToUpper(),
                new[] { "TEST", "ITEM" },
                "String transformation mapping"
            ),
            new(
                new List<TestItem> { new(5, "Five"), new(10, "Ten"), new(15, "Fifteen") }.AsReadOnly(),
                3, 1, 10,
                item => (item.Id * 2).ToString(),
                new[] { "10", "20", "30" },
                "Numeric transformation mapping"
            ),
            new(
                CreateTestItems(1),
                1000, 500, 1,
                item => $"Page500-{item.Name}",
                new[] { "Page500-Item 1" },
                "Single item with context mapping"
            )
        };

        [Theory]
        [MemberData(nameof(ComplexMappingTestCases))]
        public void Map_ComplexTransformations_WorksCorrectly(ComplexMappingTestCase testCase)
        {
            var originalList = new PaginatedList<TestItem>(testCase.Items, testCase.TotalCount, testCase.PageIndex, testCase.PageSize);

            var mappedList = originalList.Map(testCase.Mapper);

            mappedList.Items.ShouldBe(testCase.ExpectedResults);

            // Ensure pagination metadata is preserved
            mappedList.TotalCount.ShouldBe(testCase.TotalCount);
            mappedList.PageIndex.ShouldBe(testCase.PageIndex);
            mappedList.PageSize.ShouldBe(testCase.PageSize);
        }
    }
    #endregion

    #region Integration Tests
    public class IntegrationTests
    {
        public record IntegrationTestCase(
            int TotalItems,
            int PageSize,
            int TargetPageIndex,
            int ExpectedItemsOnPage,
            bool ExpectedHasPrevious,
            bool ExpectedHasNext,
            string Description
        );

        public static TheoryData<IntegrationTestCase> IntegrationTestCases => new()
        {
            new(100, 10, 1, 10, false, true, "First page of standard pagination"),
            new(100, 10, 5, 10, true, true, "Middle page of standard pagination"),
            new(100, 10, 10, 10, true, false, "Last page of standard pagination"),
            new(95, 10, 10, 5, true, false, "Last page with partial items"),
            new(1000, 25, 20, 25, true, true, "Large dataset middle page"),
            new(50, 100, 1, 50, false, false, "Page size larger than total items"),
            new(1, 1, 1, 1, false, false, "Single item single page"),
            new(0, 10, 1, 0, false, false, "Empty dataset")
        };

        [Theory]
        [MemberData(nameof(IntegrationTestCases))]
        public void PaginatedList_FullWorkflow_WorksCorrectly(IntegrationTestCase testCase)
        {
            // Create test data
            var allItems = CreateTestItems(testCase.TotalItems);
            var pageItems = allItems.Skip((testCase.TargetPageIndex - 1) * testCase.PageSize)
                                  .Take(testCase.PageSize)
                                  .ToList()
                                  .AsReadOnly();

            // Create paginated list
            var paginatedList = new PaginatedList<TestItem>(pageItems, testCase.TotalItems, testCase.TargetPageIndex, testCase.PageSize);

            // Verify basic properties
            paginatedList.Items.Count.ShouldBe(testCase.ExpectedItemsOnPage);
            paginatedList.HasPreviousPage.ShouldBe(testCase.ExpectedHasPrevious);
            paginatedList.HasNextPage.ShouldBe(testCase.ExpectedHasNext);
            paginatedList.TotalCount.ShouldBe(testCase.TotalItems);

            // Test mapping preserves everything
            var mappedList = paginatedList.Map(item => new { item.Id, UpperName = item.Name.ToUpper() });

            mappedList.Items.Count.ShouldBe(testCase.ExpectedItemsOnPage);
            mappedList.HasPreviousPage.ShouldBe(testCase.ExpectedHasPrevious);
            mappedList.HasNextPage.ShouldBe(testCase.ExpectedHasNext);
            mappedList.TotalCount.ShouldBe(testCase.TotalItems);

            // Verify mapped data integrity
            var originalArray = paginatedList.Items.ToArray();
            var mappedArray = mappedList.Items.ToArray();

            for (int i = 0; i < originalArray.Length; i++)
            {
                mappedArray[i].Id.ShouldBe(originalArray[i].Id);
                mappedArray[i].UpperName.ShouldBe(originalArray[i].Name.ToUpper());
            }
        }

        [Fact]
        public void PaginatedList_ChainedMappings_WorksCorrectly()
        {
            var items = CreateTestItems(5);
            var originalList = new PaginatedList<TestItem>(items, 100, 2, 5);

            // Chain multiple mappings
            var step1 = originalList.Map(item => new { item.Id, Upper = item.Name.ToUpper() });
            var step2 = step1.Map(item => new { item.Id, Length = item.Upper.Length });
            var step3 = step2.Map(item => $"ID:{item.Id}-LEN:{item.Length}");

            // Verify final result
            step3.Items.Count.ShouldBe(5);
            step3.TotalCount.ShouldBe(100);
            step3.PageIndex.ShouldBe(2);
            step3.PageSize.ShouldBe(5);
            step3.HasPreviousPage.ShouldBeTrue();
            step3.HasNextPage.ShouldBeTrue();

            // Verify transformation chain worked
            var results = step3.Items.ToArray();
            results[0].ShouldBe("ID:1-LEN:6"); // "ITEM 1" has length 6
        }

        [Fact]
        public void PaginatedList_WithComplexObjects_HandlesCorrectly()
        {
            var complexItems = new List<TestItem>
            {
                new(1, "Alpha"),
                new(2, "Beta"),
                new(3, "Gamma")
            }.AsReadOnly();

            var paginatedList = new PaginatedList<TestItem>(complexItems, 1000, 333, 3);

            // Map to complex DTO
            var mappedList = paginatedList.Map((item, index) => new
            {
                OriginalId = item.Id,
                OriginalName = item.Name,
                ZeroBasedIndex = index,
                OneBasedIndex = index + 1,
                IsEven = item.Id % 2 == 0,
                NameLength = item.Name.Length,
                FormattedName = $"[{index}] {item.Name} (ID: {item.Id})"
            });

            mappedList.Items.Count.ShouldBe(3);
            mappedList.TotalCount.ShouldBe(1000);
            mappedList.PageIndex.ShouldBe(333);

            var results = mappedList.Items.ToArray();
            results[0].OriginalId.ShouldBe(1);
            results[0].ZeroBasedIndex.ShouldBe(0);
            results[0].OneBasedIndex.ShouldBe(1);
            results[0].IsEven.ShouldBeFalse();
            results[0].FormattedName.ShouldBe("[0] Alpha (ID: 1)");

            results[1].OriginalId.ShouldBe(2);
            results[1].IsEven.ShouldBeTrue();
            results[1].FormattedName.ShouldBe("[1] Beta (ID: 2)");
        }
    }
    #endregion
}
