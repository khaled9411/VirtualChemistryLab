using System.Collections.Generic;

[System.Serializable]
public class ElementData
{
    public string symbol;
    public int atomsPerMolecule;
}

[System.Serializable]
public class MoleculeData
{
    public string formula;
    public List<ElementData> elements;
    public int correctCoefficient;
    public bool isReactant;
}

[System.Serializable]
public class EquationQuestion
{
    public string title;
    public string difficulty;
    public string hint;
    public List<MoleculeData> molecules;
}

public static class EquationDatabase
{
    public static List<EquationQuestion> GetAllQuestions()
    {
        return new List<EquationQuestion>
        {
            new EquationQuestion
            {
                title = "Water Formation",
                difficulty = "Easy",
                hint = "Count hydrogen atoms on both sides",
                molecules = new List<MoleculeData>
                {
                    new MoleculeData {
                        formula = "H2", isReactant = true, correctCoefficient = 2,
                        elements = new List<ElementData>{ new ElementData { symbol="H", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "O2", isReactant = true, correctCoefficient = 1,
                        elements = new List<ElementData>{ new ElementData { symbol="O", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "H2O", isReactant = false, correctCoefficient = 2,
                        elements = new List<ElementData>{
                            new ElementData { symbol="H", atomsPerMolecule=2 },
                            new ElementData { symbol="O", atomsPerMolecule=1 }
                        }
                    }
                }
            },

            new EquationQuestion
            {
                title = "Ammonia Synthesis",
                difficulty = "Easy",
                hint = "Nitrogen needs 3 hydrogen molecules",
                molecules = new List<MoleculeData>
                {
                    new MoleculeData {
                        formula = "N2", isReactant = true, correctCoefficient = 1,
                        elements = new List<ElementData>{ new ElementData { symbol="N", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "H2", isReactant = true, correctCoefficient = 3,
                        elements = new List<ElementData>{ new ElementData { symbol="H", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "NH3", isReactant = false, correctCoefficient = 2,
                        elements = new List<ElementData>{
                            new ElementData { symbol="N", atomsPerMolecule=1 },
                            new ElementData { symbol="H", atomsPerMolecule=3 }
                        }
                    }
                }
            },

            new EquationQuestion
            {
                title = "Iron Rusting",
                difficulty = "Medium",
                hint = "4 iron atoms react with 3 oxygen molecules",
                molecules = new List<MoleculeData>
                {
                    new MoleculeData {
                        formula = "Fe", isReactant = true, correctCoefficient = 4,
                        elements = new List<ElementData>{ new ElementData { symbol="Fe", atomsPerMolecule=1 } }
                    },
                    new MoleculeData {
                        formula = "O2", isReactant = true, correctCoefficient = 3,
                        elements = new List<ElementData>{ new ElementData { symbol="O", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "Fe2O3", isReactant = false, correctCoefficient = 2,
                        elements = new List<ElementData>{
                            new ElementData { symbol="Fe", atomsPerMolecule=2 },
                            new ElementData { symbol="O", atomsPerMolecule=3 }
                        }
                    }
                }
            },

            new EquationQuestion
            {
                title = "Methane Combustion",
                difficulty = "Medium",
                hint = "Count carbon, then hydrogen, then balance oxygen last",
                molecules = new List<MoleculeData>
                {
                    new MoleculeData {
                        formula = "CH4", isReactant = true, correctCoefficient = 1,
                        elements = new List<ElementData>{
                            new ElementData { symbol="C", atomsPerMolecule=1 },
                            new ElementData { symbol="H", atomsPerMolecule=4 }
                        }
                    },
                    new MoleculeData {
                        formula = "O2", isReactant = true, correctCoefficient = 2,
                        elements = new List<ElementData>{ new ElementData { symbol="O", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "CO2", isReactant = false, correctCoefficient = 1,
                        elements = new List<ElementData>{
                            new ElementData { symbol="C", atomsPerMolecule=1 },
                            new ElementData { symbol="O", atomsPerMolecule=2 }
                        }
                    },
                    new MoleculeData {
                        formula = "H2O", isReactant = false, correctCoefficient = 2,
                        elements = new List<ElementData>{
                            new ElementData { symbol="H", atomsPerMolecule=2 },
                            new ElementData { symbol="O", atomsPerMolecule=1 }
                        }
                    }
                }
            },

            new EquationQuestion
            {
                title = "Aluminium Oxide",
                difficulty = "Hard",
                hint = "Find LCM of Al atoms on both sides first",
                molecules = new List<MoleculeData>
                {
                    new MoleculeData {
                        formula = "Al", isReactant = true, correctCoefficient = 4,
                        elements = new List<ElementData>{ new ElementData { symbol="Al", atomsPerMolecule=1 } }
                    },
                    new MoleculeData {
                        formula = "O2", isReactant = true, correctCoefficient = 3,
                        elements = new List<ElementData>{ new ElementData { symbol="O", atomsPerMolecule=2 } }
                    },
                    new MoleculeData {
                        formula = "Al2O3", isReactant = false, correctCoefficient = 2,
                        elements = new List<ElementData>{
                            new ElementData { symbol="Al", atomsPerMolecule=2 },
                            new ElementData { symbol="O", atomsPerMolecule=3 }
                        }
                    }
                }
            }
        };
    }
}