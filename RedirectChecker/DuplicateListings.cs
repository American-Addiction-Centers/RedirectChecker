using System;
using System.Collections.Generic;
using System.IO;

namespace RedirectChecker {
    public class DuplicateListings {
        public DuplicateListings() {
        }

        public void FindDuplicates() {
            string[] listings = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "CR_free_listings.csv"));
            //string[] listings = File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "CR_test_info.csv"));
            List<string> duplicates = new List<string>();

            Boolean isDupe = false;

            using (StreamWriter output = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "CR_duplicates.txt"))) {
                for (int i = 1; i < listings.Length; i++) {
                    string[] listing = listings[i].Split(',');

                    if (i == 1) {
                        output.WriteLine(listings[0]);
                    }

                    for (int k = i + 1; k < listings.Length; k++) {
                        string[] nextListing = listings[k].Split(',');
                        if (listing[2].ToLower().Contains(nextListing[2].ToLower())) {
                            string subAddress = nextListing[3];
                            subAddress = subAddress.Substring(0, subAddress.Length / 3);

                            if (listing[3].ToLower().Contains(subAddress.ToLower())) {
                                if (!isDupe) {
                                    if (duplicates.Count == 0) {
                                        duplicates.Add(listings[i].ToString());
                                        duplicates.Add(listings[k].ToString());
                                    }
                                    isDupe = true;
                                }
                                else {
                                    Boolean isFound = false;
                                    for (int f = 0; f < duplicates.Count; f++) {
                                        if (duplicates[f].Contains(nextListing[1])) {
                                            isFound = true;
                                        }
                                    }

                                    if (!isFound) {
                                        duplicates.Add(listings[k].ToString());
                                    }
                                }
                            }
                        }
                    }

                    isDupe = false;
                }

                foreach (var dupe in duplicates) {
                    output.WriteLine(dupe);
                }
            }
        }
    }
}
