using UnityEngine;
using System;
using System.IO; // File operations
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


/// <summary>
/// The XmmInterface class communicates with the native plugin.
/// It provides a C# interface to the XmmTrainingSet and XmmModel classes
// which will be instantiated in a Unity script.
/// </summary>

public class XmmInterface {

  private const string LIBRARY_IMPORT_NAME = 
#if UNITY_IPHONE || UNITY_XBOX360
  "__Internal"
#else
  "XmmEngine"
#endif
  ;
  //==========================================================================//
  //==================== Interface with the native plugin ====================//
  //==========================================================================//

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "createModelInstance")]
  public static extern int createModelInstance(string modelType);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "deleteModelInstance")]
  public static extern int deleteModelInstance(int index);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setCurrentModelInstance")]
  public static extern void setCurrentModelInstance(int index);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getCurrentModelInstance")]
  public static extern int getCurrentModelInstance();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getNbOfModelInstances")]
  public static extern int getNbOfModelInstances();


  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "createSetInstance")]
  public static extern int createSetInstance();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "deleteSetInstance")]
  public static extern int deleteSetInstance(int index);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setCurrentSetInstance")]
  public static extern void setCurrentSetInstance(int index);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getCurrentSetInstance")]
  public static extern int getCurrentSetInstance();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getNbOfSetInstances")]
  public static extern int getNbOfSetInstances();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "trainModelFromSet")]
  public static extern void trainModelFromSet();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "trained")]
  public static extern int trained();

  //========================= Phrases / TrainingSets =========================//

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "addPhraseFromData")]
  public static extern void addPhraseFromData(string label, string[] colNames,
                                               IntPtr phrase, int dimIn,
                                               int dimOut, int phraseSize);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "addPhrase")]
  public static extern void addPhrase(string phrase);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getPhrase")]
  public static extern IntPtr getPhrase(int phraseIndex);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "removePhrase")]
  public static extern void removePhrase(int phraseIndex);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "removePhrasesOfLabel")]
  public static extern void removePhrasesOfLabel(string label);  

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTrainingSetSize")]
  public static extern int getTrainingSetSize();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTrainingSetNbOfLabels")]
  public static extern int getTrainingSetNbOfLabels();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTrainingSetLabels")]
  public static extern IntPtr getTrainingSetLabels();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTrainingSet")]
  public static extern IntPtr getTrainingSet();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getSubTrainingSet")]
  public static extern IntPtr getSubTrainingSet(string label);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setTrainingSet")]
  public static extern void setTrainingSet(string trainingSet);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "addTrainingSet")]
  public static extern void addTrainingSet(string trainingSet);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "clearTrainingSet")]
  public static extern void clearTrainingSet();

  //=========================== Config getters ===============================//

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getModelType")]
  public static extern int getModelType();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getBimodal")]
  public static extern int getBimodal();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getNbOfModels")]
  public static extern int getNbOfModels();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getModelLabels")]
  public static extern IntPtr getModelLabels();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getInputDimension")]
  public static extern int getInputDimension();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getOutputDimension")]
  public static extern int getOutputDimension();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getGaussians")]
  public static extern int getGaussians();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getRelativeRegularization")]
  public static extern float getRelativeRegularization();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getAbsoluteRegularization")]
  public static extern float getAbsoluteRegularization();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getCovarianceMode")]
  public static extern int getCovarianceMode();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getHierarchical")]
  public static extern int getHierarchical();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getStates")]
  public static extern int getStates();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTransitionMode")]
  public static extern int getTransitionMode();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getRegressionEstimator")]
  public static extern int getRegressionEstimator();

  //=========================== Config setters ===============================//

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setGaussians")]
  public static extern void setGaussians(int gaussians);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setRelativeRegularization")]
  public static extern void setRelativeRegularization(float relReg);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setAbsoluteRegularization")]
  public static extern void setAbsoluteRegularization(float absReg);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setCovarianceMode")]
  public static extern void setCovarianceMode(int covarianceMode);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setHierarchical")]
  public static extern void setHierarchical(int hierarchical);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setStates")]
  public static extern void setStates(int states);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setTransitionMode")]
  public static extern void setTransitionMode(int transitionMode);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setRegressionEstimator")]
  public static extern void setRegressionEstimator(int regressionEstimator);

  //============================= "Core" methods =============================//

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getLikelihoodWindow")]
  public static extern int getLikelihoodWindow();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "setLikelihoodWindow")]
  public static extern void setLikelihoodWindow(int window);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getModel")]
  public static extern IntPtr getModel();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "clearModel")]
  public static extern void clearModel();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "filter")]
  public static extern void filter(IntPtr observation, int observationSize);

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getFilteringResults")]
  public static extern IntPtr getFilteringResults();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getLikeliest")]
  public static extern IntPtr getLikeliest();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getLikelihoods")]
  public static extern IntPtr getLikelihoods();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getTimeProgressions")]
  public static extern IntPtr getTimeProgressions();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "getRegression")]
  public static extern IntPtr getRegression();

  [DllImport (LIBRARY_IMPORT_NAME, EntryPoint = "reset")]
  public static extern void reset();
}


//============================================================================//

/// <summary>
/// The XmmTrainingSet class
/// </summary>

public class XmmTrainingSet {

  private int thisIndex = -1;

  public XmmTrainingSet() {
    thisIndex = XmmInterface.createSetInstance();
  }

  ~XmmTrainingSet() {
    XmmInterface.deleteSetInstance(thisIndex);
  }

  public int GetId() {
    return thisIndex;
  }

  public void AddPhraseFromData(string label, string[] colNames, float[] phrase,
                        int dimIn, int dimOut) {
    XmmInterface.setCurrentSetInstance(thisIndex);

    IntPtr unmanagedFArray = Marshal.AllocHGlobal(phrase.Length * sizeof(float));
    Marshal.Copy(phrase, 0, unmanagedFArray, phrase.Length);

    XmmInterface.addPhraseFromData(label, colNames, unmanagedFArray,
                                   dimIn, dimOut,
                                   phrase.Length / (dimIn + dimOut));

    Marshal.FreeHGlobal(unmanagedFArray);
  }

  public void AddPhrase(string phrase) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.addPhrase(phrase);
  }

  public string GetPhrase(int phraseIndex) {
    XmmInterface.setCurrentSetInstance(thisIndex);

    IntPtr phraseIntPtr = XmmInterface.getPhrase(phraseIndex);
    return Marshal.PtrToStringAnsi(phraseIntPtr);
  }

  public void RemovePhrase(int phraseIndex) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.removePhrase(phraseIndex);
  }

  public void RemovePhrasesOfLabel(string label) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.removePhrasesOfLabel(label);
  }

  public int Size() {
    XmmInterface.setCurrentSetInstance(thisIndex);
    return XmmInterface.getTrainingSetSize();
  }

  public int GetNbOfLabels() {
    XmmInterface.setCurrentSetInstance(thisIndex);
    return XmmInterface.getTrainingSetNbOfLabels();
  }

  public string[] GetLabels() {
    XmmInterface.setCurrentSetInstance(thisIndex);

    int nLabels = XmmInterface.getTrainingSetNbOfLabels();
    string[] labels = new string[nLabels];
    IntPtr stringArrayIntPtr = XmmInterface.getTrainingSetLabels();

    for (int i = 0; i < nLabels; i++) {
      IntPtr currentIntPtr = Marshal.ReadIntPtr(stringArrayIntPtr, i * IntPtr.Size); 
      labels[i] = Marshal.PtrToStringAnsi(currentIntPtr);
    }

    return labels;
  }

  public string GetSet() {
    XmmInterface.setCurrentSetInstance(thisIndex);
    return Marshal.PtrToStringAnsi(XmmInterface.getTrainingSet());
  }

  public string GetSubSet(string label) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    return Marshal.PtrToStringAnsi(XmmInterface.getSubTrainingSet(label));
  }

  public void SetSet(string trainingSet) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.setTrainingSet(trainingSet);
  }

  public void AddSet(string trainingSet) {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.addTrainingSet(trainingSet);
  }

  public void Clear() {
    XmmInterface.setCurrentSetInstance(thisIndex);
    XmmInterface.clearTrainingSet();
  }
}

//============================================================================//

/// <summary>
/// The XmmModel class
/// </summary>

public class XmmModel {

  private int thisIndex = -1;

  public XmmModel(string modelType) {
    thisIndex = XmmInterface.createModelInstance(modelType);
  }

  ~XmmModel() {
    XmmInterface.deleteModelInstance(thisIndex);
  }

  public int GetId() {
    return thisIndex;
  }

  public void Train(XmmTrainingSet trainingSet) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setCurrentSetInstance(trainingSet.GetId());
    XmmInterface.trainModelFromSet();
  }

  public bool Trained() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.trained() == 1;
  }

  //==========================================================================//

  public int GetModelType() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getModelType();
  }

  public int GetNbOfModels() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getNbOfModels();
  }

  public string[] GetLabels() {
    XmmInterface.setCurrentModelInstance(thisIndex);

    int nLabels = XmmInterface.getNbOfModels();
    string[] labels = new string[nLabels];
    IntPtr stringArrayIntPtr = XmmInterface.getModelLabels();

    for (int i = 0; i < nLabels; i++) {
      IntPtr currentIntPtr = Marshal.ReadIntPtr(stringArrayIntPtr, i * IntPtr.Size); 
      labels[i] = Marshal.PtrToStringAnsi(currentIntPtr);
    }

    return labels;
  }

  public int GetInputDimension() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getInputDimension();
  }

  public int GetOutputDimension() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getOutputDimension();
  }

  public bool GetBimodal() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getBimodal() != 0;
  }

  public int GetGaussians() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getGaussians();
  }

  public float GetRelativeRegularization() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getRelativeRegularization();
  }

  public float GetAbsoluteRegularization() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getAbsoluteRegularization();
  }

  public int GetCovarianceMode() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getCovarianceMode();
  }

  public bool GetHierarchical() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getHierarchical() != 0;
  }

  public int GetStates() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getStates();
  }

  public int GetTransitionMode() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getTransitionMode();
  }

  public int GetRegressionEstimator() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getRegressionEstimator();
  }

  //==========================================================================//

  public void SetGaussians(int gaussians) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setGaussians(gaussians);
  }

  public void SetRelativeRegularization(float relReg) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setRelativeRegularization(relReg);
  }

  public void SetAbsoluteRegularization(float absReg) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setAbsoluteRegularization(absReg);
  }

  public void SetCovarianceMode(int covarianceMode) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setCovarianceMode(covarianceMode);
  }

  public void SetHierarchical(bool hierarchical) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setHierarchical(hierarchical ? 1 : 0);
  }

  public void SetStates(int states) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setStates(states);
  }

  public void SetTransitionMode(int transitionMode) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setTransitionMode(transitionMode);
  }

  public void SetRegressionEstimator(int regressionEstimator) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setRegressionEstimator(regressionEstimator);
  }

  //==========================================================================//

  public int GetLikelihoodWindow() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return XmmInterface.getLikelihoodWindow();
  }

  public void SetLikelihoodWindow(int window) {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.setLikelihoodWindow(window);
  }

  public string GetModel() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return Marshal.PtrToStringAnsi(XmmInterface.getModel());
  }

  public void Clear() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.clearModel();
  }

  public void Filter(float[] observation) {
    XmmInterface.setCurrentModelInstance(thisIndex);

    IntPtr intPtrFArray = Marshal.AllocHGlobal(observation.Length * sizeof(float));
    Marshal.Copy(observation, 0, intPtrFArray, observation.Length);

    XmmInterface.filter(intPtrFArray, observation.Length);

    Marshal.FreeHGlobal(intPtrFArray);
  }

  public string GetFilteringResults() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return Marshal.PtrToStringAnsi(XmmInterface.getFilteringResults());
  }

  public string GetLikeliest() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    return Marshal.PtrToStringAnsi(XmmInterface.getLikeliest());
  }

  public float[] GetLikelihoods() {
    XmmInterface.setCurrentModelInstance(thisIndex);

    int nModels = XmmInterface.getNbOfModels();
    float[] likelihoods = new float[nModels];
    IntPtr intPtrLikelihoods = XmmInterface.getLikelihoods();
    Marshal.Copy(intPtrLikelihoods, likelihoods, 0, nModels);
    return likelihoods;
  }

  public float[] GetTimeProgressions() {
    XmmInterface.setCurrentModelInstance(thisIndex);

    int nModels = XmmInterface.getNbOfModels();
    float[] timeProgressions = new float[nModels];
    IntPtr intPtrTimeProgressions = XmmInterface.getTimeProgressions();
    Marshal.Copy(intPtrTimeProgressions, timeProgressions, 0, nModels);
    return timeProgressions;
  }

  public float[] GetRegression() {
    XmmInterface.setCurrentModelInstance(thisIndex);

    int nModels = XmmInterface.getNbOfModels();
    float[] outputValues = new float[nModels];
    IntPtr intPtrOutputValues = XmmInterface.getLikelihoods();
    Marshal.Copy(intPtrOutputValues, outputValues, 0, nModels);
    return outputValues;
  }

  public void Reset() {
    XmmInterface.setCurrentModelInstance(thisIndex);
    XmmInterface.reset();
  }
}
