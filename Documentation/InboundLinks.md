# Checking for inbound links to a page

Allows members of the `WebServices` role (see [Configuring the Umbraco User Access Manager](Configuration.md)) to check if any other pages link to a selected page, so that those pages can be updated if the selected page is to be deleted.

## Data sources

### Examine

[Examine](https://our.umbraco.org/documentation/Reference/Searching/Examine/) is an Umbraco implementation of the Lucene search engine.

*	Link could be in any number of content fields. Need to define which fields.
*	Link could be in a picker control (?), linked via RTE link option, manually entered A tag, other? Therefore, the link can be in different formats such as full URL, node id, relative URL.
*	Could be multiple links from the same page (different content elements).


A new index called `NodeLinks` has been created specifically for this search, containing all content fields. It is maintained by an `ExamineEventHandler` in [Escc.Umbraco.UserAccessWebService](https://github.com/east-sussex-county-council/Escc.Umbraco.UserAccessWebService/) when content is updated. It contains a list of node Ids for pages linked to (out) from each page. 

A new custom field, `NodeLinksTo` is added in the `GatheringNodeData` Examine event which contains a list of outgoing link node Ids. See an [example of a “Gathering Node Data” function](http://thecogworks.co.uk/blog/posts/2012/november/examiness-hints-and-tips-from-the-trenches-part-2/).

The following setting in `web.config` causes `ExamineEventHandler` to update the index when saving a page:

	<appSetting>
		<add key="IsUmbracoBackOffice" value="true" />
	</appSetting>

To set up the new index, we need to create 3 items in Examine: an indexer, a searcher and an index set.

`ExamineSettings.config` is where you configure the indexer and searcher. The indexer configures the index itself, such as the analyser type and whether unpublished and protected pages are included. It is configured to include both unpublished and protected pages. The searcher uses the `Whitespace` analyser to work with the way the `NodeLinksTo` field is created.

	<Examine>
	  <ExamineIndexProviders>
	    <providers>
            <add name="NodeLinksIndexer" type="UmbracoExamine.UmbracoContentIndexer, UmbracoExamine"
	         supportUnpublished="true"
	         supportProtected="true"
	         analyzer="Lucene.Net.Analysis.WhitespaceAnalyzer, Lucene.Net"
	         indexSet="NodeLinksIndexSet" />
	    </providers>
	  </ExamineIndexProviders>
	
	  <ExamineSearchProviders defaultProvider="ExternalSearcher">
	    <providers>
	      <add name="NodeLinksSearcher" type="UmbracoExamine.UmbracoExamineSearcher, UmbracoExamine"
	           analyzer="Lucene.Net.Analysis.WhitespaceAnalyzer, Lucene.Net" indexSet="NodeLinksIndexSet" enableLeadingWildcards="true" />
	    </providers>
	  </ExamineSearchProviders>
	</Examine>

`ExamineIndex.config` is where you configure the index set. The index set declares the location of the index data (in `App_Data`), as well as the fields contained in the index.

	<ExamineLuceneIndexSets>
	  <IndexSet SetName="NodeLinksIndexSet" IndexPath="~/App_Data/TEMP/ExamineIndexes/{machinename}/NodeLinks/">
	    <IndexAttributeFields>
	      <add Name="id" />
	      <add Name="nodeName"/>
	      <add Name="nodeTypeAlias" />
	    </IndexAttributeFields>
	    <IndexUserFields>
	    </IndexUserFields>
	  </IndexSet>
	</ExamineLuceneIndexSets>

### Redirects database

The page being queried may be the target of a redirect, so it needs to be checked for in the [Escc.Redirects](https://github.com/east-sussex-county-council/Escc.Redirects) database.

Scenario – page / url A redirects to page B. when checking if page B can be deleted, need to report that A redirects to B. Otherwise requesting A would result in a 404.

### Inspyder

Inspyder is a link checker tool. Its data needs to be checked and fully understood to determine if it will be useful for this application.

Jane produced a sitewide report with all options activated. Although it is a CSV file, it is not purely data rows. In fact, it appears to have the data of multiple report types combined into one file. Therefore, the data in each column has different meaning / content. It may be that only one type of report is needed, otherwise I think each report type will need to be generated on its own.
