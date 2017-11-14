using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using SemTK_Universal_Support.SemTK.SparqlX;
using SemTK_Universal_Support.SemTK.Utility;

namespace SemTK_Universal_Support.SemTK.OntologyTools
{
    public class OntologyInfo
    {
        /*
        * OntologyInfo is a class that contains the bulk of the understanding of the actual model.
        * it represents all the classes, relationships between them (subclass relations) and related
        * properties. 
        * it provides functionality for determining pathing between two arbitrary classes, enumeration
        * values, and types for properties.
        */

        // various dictionaries to control access to model info.
        private Dictionary<String, OntologyClass> classHash = new Dictionary<string, OntologyClass>();
        private Dictionary<String, OntologyProperty> propertyHash = new Dictionary<string, OntologyProperty>();
        private Dictionary<String, List<OntologyPath>> connHash = new Dictionary<string, List<OntologyPath>>();
        private Dictionary<String, List<OntologyClass>> subclassHash = new Dictionary<string, List<OntologyClass>>();
        private Dictionary<String, List<String>> enumerationHash = new Dictionary<string, List<string>>();

        private List<String> pathWarnings = new List<string>(); // problems encountered while searching for a path.
        private static int MAXPATHLENGTH = 50;  // max number of hops any path we are willing to find may contain.
        private static int restCount = 0;
        private static long JSON_VERSION = 2;

        // used in the serialization and have to be held internally in the event that an oInfo is generated 
        // be de-serializing a json blob.
        private SparqlConnection modelConnnection;

        public OntologyInfo() { }
        
        public OntologyInfo(JsonObject json)
        {
            throw new Exception("OInfo constructor which takes json not yet implemented");
        }

        public void AddClass(OntologyClass oClass)
        {
            String classNameStr = oClass.GetNameString(false);          // get the full name of the class and do not strip the uri info
            this.connHash.Clear();

            this.classHash.Add(classNameStr, oClass);
            // store info on the related subclasses
            List<String> superClassNames = oClass.GetParentNameStrings(false); // get the parents. there may be several.
            // spin through the list and find the ones that need to be added
            foreach(String scn in superClassNames)
            {
                if (!(this.subclassHash.ContainsKey(scn)))
                {   // this superclass was not previously added.
                    List<OntologyClass> scList = new List<OntologyClass>();
                    scList.Add(oClass);
                    this.subclassHash.Add(scn, scList);
                }
                else
                {   // just add this one
                    this.subclassHash[scn].Add(oClass);
                }
            }
        }

        public Dictionary<String, List<OntologyClass>> GetSubClassHash() { return this.subclassHash;  }
        public Dictionary<String, OntologyClass> GetClassHash() { return this.classHash; }

        public List<String> GetSubclassNames(String superClassName) { return this.GetSubclassNames(superClassName, null); }
        public List<String> GetSubclassNames(String superClassName, List<String> retval)
        {
            // return a list of the subclass names, if any, for the given class.
            if(retval == null) { retval = new List<String>(); }

            if(this.subclassHash.ContainsKey(superClassName))
            {

                List<OntologyClass> subclasses = this.subclassHash[superClassName];
                // we found the key. let us use it.
                foreach(OntologyClass currSubClass in subclasses)
                {
                    retval.Add(currSubClass.GetNameString(false));  // check the existing one and add it.
                    retval = this.GetSubclassNames(currSubClass.GetNameString(false), retval); // recursively add the subclass' subclasses.
                }

            }
            else
            {
                // the key was not found. oops.
            }
            // send the results so far.
            return retval;
        }

        public List<String> GetSuperclassNames(String subClassName) { return this.GetSuperclassNames(subClassName, null); }
        public List<String> GetSuperclassNames(String subClassName, List<String> retval)
        {
            if (retval == null) { retval = new List<String>(); }

            List<OntologyClass> superclasses = new List<OntologyClass>(); 
            foreach(String currParentName in this.classHash[subClassName].GetParentNameStrings(false))
            {
                retval.Add(currParentName);
                superclasses.Add(this.classHash[currParentName]);
            }

            // get the parents' parents 
            foreach(OntologyClass currParentClass in superclasses)
            {
                retval = this.GetSuperclassNames(currParentClass.GetNameString(false), retval);
            }

            return retval;
        }

        public Boolean ContainsClass(String classNameString){ return this.classHash.ContainsKey(classNameString); }

        public List<OntologyProperty> GetInheritedProperies(OntologyClass oClass)
        {
            List<OntologyProperty> retval = new List<OntologyProperty>();
            Dictionary<String, OntologyProperty> tempRetval = new Dictionary<string, OntologyProperty>();

            // get the full list
            // walk up the parent chain and add all the properties we need.
            List<String> fullParentList = null;
            fullParentList = this.GetSuperclassNames(oClass.GetNameString(false), fullParentList);
            fullParentList.Add(oClass.GetNameString(false));

            // go through the superclass list and gather all the properties.
            foreach(String scn in fullParentList)
            {
                // add each property from the superclass
                foreach (OntologyProperty currProp in this.classHash[scn].GetProperties())
                {
                    tempRetval.Add(currProp.GetNameStr(), currProp);
                }
            }

            // assemble into a single list with no repeated values
            Object[] keys = tempRetval.Keys.ToArray();
            Array.Sort(keys);

            foreach(Object propKey in keys) { retval.Add(tempRetval[(String)propKey]); }

            return retval;
        }

        public List<OntologyProperty> GetDescendantPropeties(OntologyClass oClass)
        {
            List<OntologyProperty> retval = new List<OntologyProperty>();
            Dictionary<String, OntologyProperty> tempRetval = new Dictionary<string, OntologyProperty>();

            // get the full list...
            // walk up the parent chain and add all the properties
            List<String> fullChildList = null;
            fullChildList = this.GetSubclassNames(oClass.GetNameString(false), fullChildList);

            // go through the subclass list and gather all the properties
            foreach(String scn in fullChildList)
            {   // add each property from the child class
                foreach(OntologyProperty currProp in this.classHash[scn].GetProperties() ){
                    tempRetval.Add(currProp.GetNameStr(), currProp);
                }
            }
            // assemble into a single list without repeated values
            foreach(String propKey in tempRetval.Keys)
            {
                retval.Add(tempRetval[propKey]);
            }
            // send it out
            return retval;

        }

        public OntologyProperty GetInheritedPropertyByKeyName(OntologyClass ontClass, String propName)
        {
            OntologyProperty retval = null;
            List<OntologyProperty> props = this.GetInheritedProperies(ontClass);

            foreach(OntologyProperty i in props)
            {
                if(i.GetNameStr(true).Equals(propName))
                {
                    retval = i;
                    break;
                }
            }

            return retval;
        }

        public List<String> GetPathWarnings() { return this.pathWarnings; }
        public List<OntologyPath> GetConnList(String classNameStr)
        {
            // return or calculate all legal one-hop paths to and from a class
            if (!this.connHash.ContainsKey(classNameStr))
            {   // we need to calculate the connection list.
                List<OntologyPath> ret = new List<OntologyPath>();
                OntologyPath path;

                if (!this.classHash.ContainsKey(classNameStr))
                {   // couldn't find it. panic.
                    throw new Exception("Internal error in OntologyInfo.getConnList(): class name is not in the ontology: " + classNameStr);
                }

                OntologyClass classVal = this.classHash[classNameStr];
                Dictionary<String, int> foundHash = new Dictionary<string, int>();
                String hashStr = "";

                // calculate the needed values.
                List<OntologyProperty> props = this.GetInheritedProperies(classVal);
                foreach (OntologyProperty prop in props)
                {
                    String rangeClassName = prop.GetRangeStr();

                    // if the range looks right
                    if (this.ContainsClass(rangeClassName))
                    {
                        // exact match classA -> hasA -> rangeClassname
                        path = new OntologyPath(classNameStr);
                        path.AddTriple(classNameStr, prop.GetNameStr(), rangeClassName);
                        hashStr = path.AsString();

                        if (!foundHash.ContainsKey(hashStr))
                        {
                            ret.Add(path);
                            foundHash.Add(hashStr, 1);
                        }

                        // sub-classes: class -> hasA -> subclass(Rangename)
                        List<String> rangeSubNames = this.GetSubclassNames(rangeClassName);
                        foreach (String jString in rangeSubNames)
                        {
                            if (this.ContainsClass(jString))
                            {
                                path = new OntologyPath(classNameStr);
                                path.AddTriple(classNameStr, prop.GetNameStr(), jString);
                                hashStr = path.AsString();
                                if (!foundHash.ContainsKey(hashStr)) { foundHash.Add(hashStr, 1); }
                            }
                        }
                    }
                }

                // -- calculate HadBy: class which HasA classNameStr
                // store all superclasses of target class
                List<String> supList = this.GetSuperclassNames(classNameStr);

                foreach (String cname in this.classHash.Keys)
                {
                    // loop through every property 
                    List<OntologyProperty> cprops = this.GetInheritedProperies(this.classHash[cname]);

                    foreach (OntologyProperty prop in cprops)
                    {
                        String rangeClassStr = prop.GetRangeStr();

                        // HadBy : cName -> hasA -> class
                        if (rangeClassStr.Equals(classNameStr))
                        {
                            path = new OntologyPath(classNameStr);
                            path.AddTriple(cname, prop.GetNameStr(), classNameStr);
                            hashStr = path.AsString();
                            if (!foundHash.ContainsKey(hashStr))
                            {
                                ret.Add(path);
                                foundHash.Add(hashStr, 1);
                            }
                        }

                        // IsA + hadBy : cName -> hasA -> superClasss (class)
                        for(int j = 0; j < supList.Count(); j++)
                        {
                            if (rangeClassStr.Equals(supList[j]))
                            {
                                path = new OntologyPath(classNameStr);
                                path.AddTriple(cname, prop.GetNameStr(), classNameStr);
                                hashStr = path.AsString();

                                if(!foundHash.ContainsKey(hashStr))
                                {
                                    ret.Add(path);
                                    foundHash.Add(hashStr, 1);
                                }
                            }
                        }
                    }
                }
                this.connHash.Add(classNameStr, ret);
            }

           return this.connHash[classNameStr];
        }

        public int GetNumberOfClasses() { return this.classHash.Count(); }

        /**
         * Returns an instance of OntologyClass for a given URI. 
         * if the OntologyInfo object has no entry for the URI, null is returned.
         **/
        public OntologyClass GetClass(String fullUriName)
        {
            OntologyClass retval = null;
            if(this.classHash.ContainsKey(fullUriName)) { retval = this.classHash[fullUriName]; }
            return retval;
        }
        /**
	    * return all the known class names
	    **/
        public List<String> GetClassNames()
        {   // return the names of the all the classes we know of
            List<String> retval = new List<String>();
            retval.AddRange(this.classHash.Keys);
            return retval;
        }

        /** for a given class, return all parent classes **/
        public List<OntologyClass> GetClassParents(OntologyClass currentClass)
        {
            List<OntologyClass> retval = new List<OntologyClass>();
            // add each parent 
            foreach(String parentNameString in currentClass.GetParentNameStrings(false))
            {
                retval.Add(this.GetClass(parentNameString));
            }


            // return the collection
            return retval;
        }

        /** return all property names **/
        public List<String> GetPropertyNames()
        {
            List<String> retval = new List<String>();
            retval.AddRange(this.propertyHash.Keys);
            return retval;
        }

        /**
	    * returns true/false value of whether a property is known to the OntologyInfo object
	     **/
        public Boolean ContainsProperty(String propertynameString)
        {
            return this.propertyHash.ContainsKey(propertynameString);
        }

        /**
        * returns the count of enumerated types known to the OntologyInfo object
        **/
        public int GetNumberOfEnum()
        {
            return this.enumerationHash.Count();
        }

        /**
	     * return count of properties known to the OntologyInfo object.
	    **/
        public int GetNumberOfProperties()
        {
            return this.propertyHash.Count();
        }


        /*  returns true/false indicating whether the classCompared is a subclass of the classComparedTo */
        public Boolean ClassIsA(OntologyClass classCompared, OntologyClass classComparedTo)
        {
            Boolean retval = false;
            // is the ClassCompared an ClassComparedTo? check recursively....
            if(classCompared == null || classComparedTo == null) {  /* do nothing */
    }
            else
            {
                // get all of the needed parents
                List<OntologyClass> allParents = this.GetClassParents(classCompared);
                allParents.Add(classCompared);

                foreach(OntologyClass currentAncestor in allParents)
                {
                    if (currentAncestor.GetNameString(false).ToLower().Equals(classComparedTo.GetNameString(false).ToLower()))
                    {
                        retval = true;
                        break;
                    }
                }
            }
            return retval;
        }

        /* checks if the classToCheck is in the given range */
        public Boolean ClassIsInRange(OntologyClass classToCheck, OntologyRange range)
        {
            Boolean retval = false;
            retval = this.ClassIsA(classToCheck, this.classHash[range.GetFullName()]);
            return retval;
        }

        public List<OntologyPath> FindAllPaths(String FromClassName, List<String> targetClassNames, String domain)
        {
            this.pathWarnings = new List<String>();
            List<OntologyPath> waitingList = new List<OntologyPath>();
            waitingList.Add(new OntologyPath(FromClassName));
            List<OntologyPath> ret = new List<OntologyPath>();
            Dictionary<String, int> targetHash = new Dictionary<String, int>(); // hash of all possible ending classes:  targetHash[className] = 1

            long t0 = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            int LENGTH_RANGE = 2;
            int SEARCH_TIME_MSEC = 5000;
            int LONGEST_PATH = 10;
          
            // return if there is no endpoint
            if(targetClassNames.Count == 0) { return ret;  }

            // set up targetHash[targetClass] = 1
            for(int i = 0; i < targetClassNames.Count; i++)
            {
                if (!targetHash.ContainsKey(targetClassNames[i])) { targetHash.Add(targetClassNames[i], 1); }
            }

            // STOP CRITERIA A: search as long as there is a waiting list
            while( waitingList.Count != 0)
            {
                // pull one off the waiting list
                OntologyPath item = waitingList[0];
                waitingList.RemoveAt(0);
                String waitClass = item.GetEndClassName();
                OntologyPath waitPath = item;

                // STOP CRITERIA B: also stop searching if:
                // this is the final path (with 1 added connection) will be longer than the first (shortest) already found path
                if (!(ret.Count == 0) && (waitPath.GetLength() + 1 > ret[0].GetLength() + LENGTH_RANGE)) { break; }

                // STOP CRITERIA C: stop if the path is too long.
                if(waitPath.GetLength() > LONGEST_PATH) { break;  }

                // STOP CRITERIA D: too much time spent searching.
                long tt = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (tt - t0 > SEARCH_TIME_MSEC)
                {
                    this.pathWarnings.Add("Note: Path-finding timing out.  Search incomplete.");
                }

                // get all of the one-hop connections and loop through them.
                List<OntologyPath> conn = this.GetConnList(waitClass);
                foreach(OntologyPath currPath in conn)
                {
                    // each connection is a path with only one node (the 0th)
                    // grab the name of the newly-found class

                    String newClass = "";
                    OntologyPath newPath = null;
                    Boolean loopFlag = false;

                    // if the newfound class is pointed to by an attribute of one on the wait list
                    if(currPath.GetStartClassName().ToLower().Equals(waitClass.ToLower())) { newClass = currPath.GetEndClassName(); }
                    else { newClass = currPath.GetStartClassName(); }

                    // check for loops in the path before adding the class
                    if( waitPath.ContainsClass(newClass) ) { loopFlag = true; }

                    // build the new path.
                    Triple t = currPath.GetTriple(0);
                    newPath = waitPath.DeepCopy();
                    newPath.AddTriple(t.GetSubject(), t.GetPredicate(), t.GetObject() );

                    // if the path leads anywhere in domain, store it.
                    OntologyName name = new OntologyName(newClass);
                    if(name.IsInDomain(domain))
                    {   // if the path leads to a target, push onth the ret list
                        if (targetHash.ContainsKey(newClass))
                        {
                            ret.Add(newPath);
                        }
                        else if(loopFlag == false)
                        {   // try extending an already found path.
                            waitingList.Add(newPath);
                        }
                    }

                }
            }
            return ret;
        }

        // -------------------------------------------------------------------------------------- serialize and deserialize json representation.
        /* the To and From json methods are adapted from the java code in oInfo. the operative mechanism is different as this code does not
         * have to be compatible with the loading from sparql methods. instead this goes further, taking advantage of the one-hop information
         * and the sub-class/super-class info buried in the exported json. 
         */

        public JsonObject ToJson()
        {
            // based on th advanced client json in the java version.
            // return the advanced client Json format...
            JsonObject retval = new JsonObject();

            // create all the prefix information...
            Dictionary<String, String> prefixes = new Dictionary<String, String>();

            // create the enumeration information
            JsonArray enumerations = new JsonArray();

            foreach (String key in this.enumerationHash.Keys)
            {   // find each and add them as needed.
                String uri = Utility.Utility.PrefixUri(key, prefixes);

                JsonObject currEnumeration = new JsonObject();
                JsonArray values = new JsonArray();         // get all the values.

                foreach (String k in this.enumerationHash[key])  // get eh enumerations we care about.
                {
                    String kUri = Utility.Utility.PrefixUri(k, prefixes);
                    values.Add(JsonValue.CreateStringValue(kUri));
                }

                currEnumeration.Add("fullUri", JsonValue.CreateStringValue(uri));
                currEnumeration.Add("enumeration", values);

                enumerations.Add(currEnumeration);
            }

            // create the propertyList information:
            JsonArray propertyList = new JsonArray();

            foreach (String key in this.propertyHash.Keys)
            {
                String uri = Utility.Utility.PrefixUri(key, prefixes);

                JsonObject currProperty = new JsonObject();
                JsonArray domain = new JsonArray();
                JsonArray range = new JsonArray();
                JsonArray labels = new JsonArray();
                JsonArray comments = new JsonArray();

                OntologyProperty currProp = this.propertyHash[key];

                // get the domain:
                foreach (String oClassKey in this.classHash.Keys)
                {   // get the classes which are in the domain
                    OntologyClass oClass = this.classHash[oClassKey];

                    if (oClass.GetProperty(key) != null)
                    {   // we found one. as a result, this will be prefixed and added.
                        OntologyProperty currPropInstance = oClass.GetProperty(key);
                        String classId = Utility.Utility.PrefixUri(oClassKey, prefixes);
                        domain.Add(JsonValue.CreateStringValue(classId));
                    }
                }

                // get the range. right now, SemTK core only supports a single value for a range, so it may seems silly/wasteful/stupid
                // to support these as an array but we intend on changing this limitation one day, so better to be prepared.
                String rangeId = Utility.Utility.PrefixUri(currProp.GetRange().GetFullName(), prefixes);
                range.Add(JsonValue.CreateStringValue(rangeId));

                // add the labels.
                foreach (String label in currProp.GetAnnotationLabels())
                {
                    labels.Add(JsonValue.CreateStringValue(label));
                }

                // add the comments
                foreach (String comment in currProp.GetAnnotationComments())
                {
                    comments.Add(JsonValue.CreateStringValue(comment));
                }

                // add all the subcomponents of the current property
                currProperty.Add("fullUri", JsonValue.CreateStringValue(uri));
                currProperty.Add("domain", domain);
                currProperty.Add("range", range);
                currProperty.Add("labels", labels);
                currProperty.Add("comments", comments);

                // add to the outgoing list
                propertyList.Add(currProperty);
            }

            // create the classList information.
            JsonArray classList = new JsonArray();
            foreach (String key in this.classHash.Keys)
            {
                String uri = Utility.Utility.PrefixUri(key, prefixes);

                JsonObject currClass = new JsonObject();
                OntologyClass oClass = this.classHash[key];

                JsonArray labels = new JsonArray();
                JsonArray comments = new JsonArray();
                JsonArray superClasses = new JsonArray();
                JsonArray subClasses = new JsonArray();
                JsonArray directConnections = new JsonArray();

                // labels
                foreach (String label in oClass.GetAnnotationLabels()) { labels.Add(JsonValue.CreateStringValue(label)); }

                // comments
                foreach (String comment in oClass.GetAnnotationComments()) { comments.Add(JsonValue.CreateStringValue(comment)); }

                // superclasses
                foreach (String parent in oClass.GetParentNameStrings(false))
                {
                    String prefixedParent = Utility.Utility.PrefixUri(parent, prefixes);
                    superClasses.Add(JsonValue.CreateStringValue(prefixedParent));
                }

                // subclasses
                if (this.subclassHash.ContainsKey(key))
                {   // only do this if any exist.
                    List<OntologyClass> myChildren = this.subclassHash[key];
                    foreach (OntologyClass currChild in myChildren)
                    {
                        String childId = Utility.Utility.PrefixUri(currChild.GetNameString(false), prefixes);
                        subClasses.Add(JsonValue.CreateStringValue(childId));
                    }
                }

                // direct connections -- generate the connections.
                foreach (OntologyPath currPath in this.GetConnList(key))
                {
                    JsonObject pathJsonObject = new JsonObject();

                    pathJsonObject.Add("startClass", JsonValue.CreateStringValue(Utility.Utility.PrefixUri(currPath.GetTriple(0).GetSubject(), prefixes)));
                    pathJsonObject.Add("predicate", JsonValue.CreateStringValue(Utility.Utility.PrefixUri(currPath.GetTriple(0).GetPredicate(), prefixes)));
                    pathJsonObject.Add("destinationClass", JsonValue.CreateStringValue(Utility.Utility.PrefixUri(currPath.GetTriple(0).GetObject(), prefixes)));

                    directConnections.Add(pathJsonObject);
                }

                // full uri information
                currClass.Add("fullUri", JsonValue.CreateStringValue(uri));

                // add everything else to the outgoing structure.
                currClass.Add("superClasses", superClasses);
                currClass.Add("subClasses", subClasses);
                currClass.Add("comments", comments);
                currClass.Add("labels", labels);
                currClass.Add("directConnections", directConnections);

                classList.Add(currClass);
            }

            // create the prefix array
            JsonArray prefixList = new JsonArray();

            foreach(String k in prefixes.Keys)
            {
                JsonObject pref = new JsonObject();
                pref.Add("prefixId", JsonValue.CreateStringValue(prefixes[k]));
                pref.Add("prefix", JsonValue.CreateStringValue(k));
            }

            String creationTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            retval.Add("version", JsonValue.CreateNumberValue(OntologyInfo.JSON_VERSION));
            retval.Add("generated", JsonValue.CreateStringValue(creationTime));

            if(this.modelConnnection != null) { retval.Add("sparqlConn", this.modelConnnection.ToJson()); }

            JsonObject jsonOInfo = new JsonObject();
            jsonOInfo.Add("prefixes", prefixList);
            jsonOInfo.Add("enumerations", enumerations);
            jsonOInfo.Add("propertyList", propertyList);
            jsonOInfo.Add("classList", classList);

            // add the oInfo serialization to the outgoing object.
            retval.Add("ontologyInfo", jsonOInfo);

            // ship it out.
            return retval;
        }

        public void AddJson(JsonObject encodedOInfo)
        {
            // check the version:
            long version = 0;
            if(encodedOInfo.ContainsKey("version")) { version = (long)encodedOInfo.GetNamedNumber("version"); }
            // check the version information now
            if(version > OntologyInfo.JSON_VERSION)
            {   // fail gracelessly
                throw new Exception("Can't decode OntologyInfo JSON with newer version > " + OntologyInfo.JSON_VERSION + " : found " + version);
            }

            // get the connection, if it exists and we care.
            if (encodedOInfo.ContainsKey("sparqlConn"))
            {   // set it up. 
                JsonObject connObj = encodedOInfo.GetNamedObject("sparqlConn");
                SparqlConnection connVal = new SparqlConnection(connObj.ToString());
                this.modelConnnection = connVal;
            }

            // get the oInfo block
            if (!encodedOInfo.ContainsKey("ontologyInfo"))
            {   // panic
                throw new Exception("encoded group does not include the ontologyInfo block and cannot be used.");
            }
            JsonObject oInfoBlock = encodedOInfo.GetNamedObject("ontologyInfo");

            // unpack the prefixews
            Dictionary<String, String> prefixHash = new Dictionary<String, String>();
            JsonArray prefixes = oInfoBlock.GetNamedArray("prefixes");

            for(int i = 0; i < prefixes.Count; i++)
            {
                JsonObject currPrefix = prefixes.GetObjectAt((uint)i);
                prefixHash.Add(currPrefix.GetNamedString("prefixId"), currPrefix.GetNamedString("prefix"));
            }

            // unpack everything else. this is a little different than in the java because we do not have to use the table load
            // logic. as a result, this may be more straightforward.

            // unpack the classes:

            JsonArray classArr = oInfoBlock.GetNamedArray("classList");

            for(int m = 0; m < classArr.Count; m++)
            {   // get each class and add them... and related values...

                JsonObject currClass = classArr.GetObjectAt((uint)m);
                String fullUri = Utility.Utility.UnPrefixUri(currClass.GetNamedString("fullUri"), prefixHash);
                JsonArray subC = currClass.GetNamedArray("subClasses");
                JsonArray superC = currClass.GetNamedArray("superClasses");
                JsonArray comments = currClass.GetNamedArray("comments");
                JsonArray labels = currClass.GetNamedArray("labels");
                JsonArray direct = currClass.GetNamedArray("directConnections");

                // find or create the class.
                OntologyClass curr = null;

                if (this.classHash.ContainsKey(fullUri))
                {   // get that one.
                    curr = classHash[fullUri];
                }
                else
                {   // create a new one
                    curr = new OntologyClass(fullUri);
                    this.classHash.Add(fullUri, curr);
                }

                // create the List of parent names
                for (int pi = 0; pi < superC.Count; pi++)
                {   // check and add the superclasses
                    String pn = Utility.Utility.UnPrefixUri(superC.GetStringAt((uint)pi), prefixHash);
                    // check that the parent exists. if not, make it. 
                    if (!this.classHash.ContainsKey(pn))
                    {
                        OntologyClass parentClass = new OntologyClass(pn);
                        classHash.Add(pn, parentClass);
                    }

                    // add the parent.
                    curr.AddParentName(pn);
                }
                
                // get the comments.
                for (int com = 0; com < comments.Count; com++)
                {
                    String c = comments.GetStringAt((uint)com);
                    curr.AddAnnotationComment(c);
                }

                // get the labels.
                for(int lab = 0; lab < labels.Count; lab++)
                {
                    String l = labels.GetStringAt((uint)lab);
                    curr.AddAnnotationLabel(l);
                }

                // add the subclasses as well...
                List<OntologyClass> children = new List<OntologyClass>();
                for(int h = 0; h < subC.Count; h++)
                {   // check and add the subclasses
                    String cn = Utility.Utility.UnPrefixUri(subC.GetStringAt((uint)h), prefixHash);
                    OntologyClass child = null;
                    // check that it exists. if not, make it.
                    if (!this.classHash.ContainsKey(cn))
                    {
                        OntologyClass childClass = new OntologyClass(cn);
                        classHash.Add(cn, childClass);
                        child = childClass;
                    }
                    else
                    {
                        child = this.classHash[cn];
                    }
                    children.Add(child);
                }

                // if there were any entries, add this.
                if(children.Count > 0) { this.subclassHash.Add(fullUri, children); }

                // handle the connections.
                List<OntologyPath> paths = new List<OntologyPath>();
                for(int b = 0; b < direct.Count; b++)
                {   // all the one-hop connections.
                    JsonObject jdir = direct.GetObjectAt((uint)b);
                    String destinationClassUri = Utility.Utility.UnPrefixUri(jdir.GetNamedString("destinationClass"), prefixHash);
                    String predicateUri = Utility.Utility.UnPrefixUri(jdir.GetNamedString("predicate"), prefixHash);
                    String startClassUri = Utility.Utility.UnPrefixUri(jdir.GetNamedString("startClass"), prefixHash);

                    OntologyPath op = new OntologyPath(startClassUri);
                    op.AddTriple(startClassUri, predicateUri, destinationClassUri);

                    // add to the list
                    paths.Add(op);
                }

                if(paths.Count > 0)
                {   // we have some paths. add it.
                    this.connHash.Add(fullUri, paths);
                }

                // done with the classes.
            }

            // unpack the properties:

            JsonArray propertyArr = oInfoBlock.GetNamedArray("propertyList");

            for(int j = 0; j < propertyArr.Count; j++)
            {   // add each entry to the property list.
                JsonObject currPropJson = propertyArr.GetObjectAt((uint)j);

                String fullUri = Utility.Utility.UnPrefixUri(currPropJson.GetNamedString("fullUri"), prefixHash);
                JsonArray comments = currPropJson.GetNamedArray("comments");
                JsonArray labels = currPropJson.GetNamedArray("labels");
                JsonArray domain = currPropJson.GetNamedArray("domain");
                JsonArray range = currPropJson.GetNamedArray("range");

                // get the (currently) single range.
                String rangeUri = Utility.Utility.UnPrefixUri(range.GetStringAt(0), prefixHash);
                // create the property
                OntologyProperty oProp = new OntologyProperty(fullUri, rangeUri);

                for(int di = 0; di < domain.Count; di++)
                {   // get the domain info and add it.
                    String domainUri = Utility.Utility.UnPrefixUri(domain.GetStringAt((uint)di), prefixHash);
                    // get the class in the domain.
                    OntologyClass domClass = this.classHash[domainUri];
                    domClass.AddProperty(oProp);
                }

                // add the labels.
                for(int li = 0; li < labels.Count; li++)
                {
                    String label = labels.GetStringAt((uint)li);
                    oProp.AddAnnotationLabel(label);
                }

                // add the comments
                for(int ci = 0; ci < comments.Count; ci++)
                {
                    String comment = comments.GetStringAt((uint)ci);
                    oProp.AddAnnotationComment(comment);
                }

                this.propertyHash.Add(fullUri, oProp);
            }

            // add the enumerations
            JsonArray enumerationArr = oInfoBlock.GetNamedArray("enumerations");

            for(int ei = 0; ei < enumerationArr.Count; ei++)
            {
                JsonObject enumVal = enumerationArr.GetObjectAt((uint)ei);

                String fullUri = Utility.Utility.UnPrefixUri(enumVal.GetNamedString("fullUri"), prefixHash);
                JsonArray children = enumVal.GetNamedArray("enumeration");

                List<String> childUris = new List<String>();
                for(int ci = 0; ci < children.Count; ci++)
                {
                    childUris.Add(Utility.Utility.UnPrefixUri(children.GetStringAt((uint)ci), prefixHash));
                }
                // add the list
                if(childUris.Count > 0)
                {
                    this.enumerationHash.Add(fullUri, childUris);
                }

            }

        }
    }

}
