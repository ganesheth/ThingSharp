using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Types;
using VDS.RDF;
using VDS.RDF.Parsing;

namespace ThingSharp.Utils
{
    public class ThingModelParser
    {
        public static Thing FromRDF(String rdfFilePath, String modelName, Uri uri)
        {
            IGraph g = new Graph();
            FileLoader.Load(g, rdfFilePath);
            // http://www.w3.org/1999/02/22-rdf-syntax-ns#type , http://www.siemens.com/bt/ontology#ThingModel
            INode modelNode = null;
            INode typeNode = null;
            INode hasProperty = null;

            modelNode = g.Nodes.Where(pn => ((UriNode)pn).Uri.Fragment == "#ThingModel").FirstOrDefault();
            typeNode = g.Triples.PredicateNodes.Where(pn => ((UriNode)pn).Uri.Fragment == "#type").FirstOrDefault();
            hasProperty = g.Triples.PredicateNodes.Where(pn => ((UriNode)pn).Uri.Fragment == "#hasProperty").FirstOrDefault();

            var triples = g.GetTriplesWithPredicateObject(typeNode, modelNode);
            INode requiredModel = null;
            requiredModel = triples.Where(tp => ((UriNode)(tp.Subject)).Uri.Fragment == modelName).FirstOrDefault().Subject;
            var properties = g.GetTriplesWithSubjectPredicate(requiredModel, hasProperty);

            Thing newThing = new Thing(uri);
            foreach (Triple p in properties)
            {
                UriNode propertyNode = (UriNode)p.Object;
                var propertyAttributes = g.GetTriplesWithSubject(propertyNode);
                string valueType = "", name = "";
                foreach (Triple pa in propertyAttributes)
                {
                    UriNode attr = pa.Predicate as UriNode;
                    if (attr.Uri.Fragment == "#hasValueType")
                    {
                        valueType = (pa.Object as LiteralNode).Value;
                    }
                    if (attr.Uri.Fragment == "#hasName")
                    {
                        name = (pa.Object as LiteralNode).Value;
                    }

                }

                if (valueType == "xsd:float")
                    newThing.Properties.Add(new Property<float>(uri, name) { ValueType = typeof(float) });
                else if (valueType == "xsd:bool")
                    newThing.Properties.Add(new Property<bool>(uri, name) { ValueType = typeof(bool) });
                else
                    newThing.Properties.Add(new Property<object>(uri, name) { ValueType = typeof(object) });
            }
            return newThing;
        }
    }
}
